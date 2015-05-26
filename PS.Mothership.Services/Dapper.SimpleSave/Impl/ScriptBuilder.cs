﻿using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Dapper.SimpleSave.Impl {
    public class ScriptBuilder {
        private readonly DtoMetadataCache _dtoMetadataCache;

        public ScriptBuilder(DtoMetadataCache dtoMetadataCache)
        {
            _dtoMetadataCache = dtoMetadataCache;
        }

        public IList<Script> Build(IEnumerable<BaseCommand> commands)
        {
            var scripts = new List<Script>();
            BuildInternal(commands, scripts);
            return scripts;
        }

        public void BuildInternal(IEnumerable<BaseCommand> commands, IList<Script> scripts)
        {
            int parmIndex = 0;
            Script script = null;
            foreach (var command in commands)
            {
                if (null == script)
                {
                    script = new Script();
                    scripts.Add(script);
                }

                if (command is UpdateCommand)
                {
                    AppendUpdateCommand(script, command as UpdateCommand, ref parmIndex);
                }
                else if (command is InsertCommand)
                {
                    AppendInsertCommand(script, command as InsertCommand, ref parmIndex);

                    script.Buffer.Append(@"
SELECT SCOPE_IDENTITY();
");

                    script = null;
                }
                else if (command is DeleteCommand)
                {
                    AppendDeleteCommand(script, command as DeleteCommand, ref parmIndex);
                }
            }
        }

        private static void AppendUpdateCommand(Script script, UpdateCommand command, ref int parmIndex)
        {
            script.Buffer.Append(string.Format(@"UPDATE {0}
SET ", command.TableName));

            int count = 0;
            foreach (var operation in command.Operations)
            {
                if (count > 0)
                {
                    script.Buffer.Append(@",
    ");
                }
                script.Buffer.Append(string.Format(@"[{0}] = ", operation.ColumnName));
                FormatWithParm(script, "{0}", ref parmIndex, operation.Value);
                ++count;
            }

            script.Buffer.Append(string.Format(@"
WHERE [{0}] = ", command.PrimaryKeyColumn));
            FormatWithParm(script, @"{0};
", ref parmIndex, new Func<object>(() => command.PrimaryKey));
            //GetPossiblyUnknownPrimaryKeyValue(command.PrimaryKey));
        }

        private static object GetPossiblyUnknownPrimaryKeyValue(int? primaryKey)
        {
            return null == primaryKey ? (object) "@insertedPk" : primaryKey;
        }

        private static void AppendDeleteCommand(Script script, DeleteCommand command, ref int parmIndex)
        {
            var operation = command.Operation;
            if (operation.ValueMetadata != null)
            {
                if (null != operation.OwnerPropertyMetadata
                    && operation.OwnerPropertyMetadata.HasAttribute<ManyToManyAttribute>())
                {
                    //  Remove record in link table; don't touch either entity table

                    script.Buffer.Append(string.Format(
                        @"DELETE FROM {0}
WHERE [{1}] = ", 
                        operation.OwnerPropertyMetadata.GetAttribute<ManyToManyAttribute>().LinkTableName,
                        operation.OwnerPrimaryKeyColumn));
                    FormatWithParm(script, "{0} AND ", ref parmIndex, operation.OwnerPrimaryKey);
                    script.Buffer.Append(string.Format("[{0}] = ", operation.ValueMetadata.PrimaryKey.Prop.Name));
                    FormatWithParm(script, @"{0};
", ref parmIndex, operation.ValueMetadata.GetPrimaryKeyValue(operation.Value));
                }
                else if (null == operation.OwnerPropertyMetadata
                    || (operation.OwnerPropertyMetadata.HasAttribute<OneToManyAttribute>()
                    && !operation.ValueMetadata.HasAttribute<ReferenceDataAttribute>()))
                {
                    //  DELETE the value from the other table

                    script.Buffer.Append(string.Format(
                        @"DELETE FROM {0}
WHERE [{1}] = ",
                        operation.ValueMetadata.TableName,
                        operation.ValueMetadata.PrimaryKey.Prop.Name));
                    FormatWithParm(script, @"{0};
", ref parmIndex, operation.ValueMetadata.GetPrimaryKeyValue(operation.Value));
                }
            }
            else
            {
                throw new ArgumentException(
                    string.Format(
                        "Invalid DELETE command: {0}",
                        JsonConvert.SerializeObject(command)),
                    "command");
            }
        }

        private void AppendInsertCommand(Script script, InsertCommand command, ref int parmIndex) {
            var operation = command.Operation;
            if (operation.ValueMetadata != null) {
                if (null != operation.OwnerPropertyMetadata
                    && operation.OwnerPropertyMetadata.HasAttribute<ManyToManyAttribute>()) {
                    //  INSERT record in link table; don't touch either entity table

                    script.Buffer.Append(string.Format(
                        @"INSERT INTO {0} (
    [{1}], [{2}]
) VALUES (
    ",
                        operation.OwnerPropertyMetadata.GetAttribute<ManyToManyAttribute>().LinkTableName,
                        operation.OwnerPrimaryKeyColumn,
                        operation.ValueMetadata.PrimaryKey.Prop.Name));
                        FormatWithParm(script, @"{0}, {1}
);
",
                            ref parmIndex,
                            operation.OwnerPrimaryKey,
                            new Func<object>(() => operation.ValueMetadata.GetPrimaryKeyValue(operation.Value)));
                        //GetPossiblyUnknownPrimaryKeyValue(operation.ValueMetadata.GetPrimaryKeyValue(operation.Value)));
                    }
                else if (null == operation.OwnerPropertyMetadata
                    || (operation.OwnerPropertyMetadata.HasAttribute<OneToManyAttribute>()
                    && !operation.ValueMetadata.HasAttribute<ReferenceDataAttribute>())) 
                {
                    //  INSERT the value into the other table
                    
                    var colBuff = new StringBuilder();
                    var valBuff = new StringBuilder();
                    var values = new ArrayList();
                    var index = 0;

                    foreach (var property in operation.ValueMetadata.Properties)
                    {
                        if (property.IsPrimaryKey)
                        {
                            continue;   //  TODO: return PK from script and associate with object
                        }

                        var getter = property.Prop.GetGetMethod();

                        if (getter == null || property.HasAttribute<ManyToManyAttribute>() || ! property.IsSaveable)
                        {
                            continue;
                        }

                        AppendPropertyToInsertStatement(colBuff, valBuff, property, ref index, operation, values, getter);
                    }

                    script.Buffer.Append(string.Format(
                    @"INSERT INTO {0} (
    {1}
) VALUES (
    ",
                        operation.ValueMetadata.TableName,
                        colBuff));
                    FormatWithParm(script, valBuff.ToString(), ref parmIndex, values.ToArray());
                    script.Buffer.Append(@"
);
");
                    script.InsertedValue = operation.Value;
                    script.InsertedValueMetadata = operation.ValueMetadata;
                }
            }
            else
            {
                throw new ArgumentException(
                    string.Format(
                        "Invalid INSERT command: {0}",
                        JsonConvert.SerializeObject(command)),
                    "command");
            }
        }

        private void AppendPropertyToInsertStatement(StringBuilder colBuff, StringBuilder valBuff, PropertyMetadata property,
            ref int index, BaseInsertDeleteOperation operation, ArrayList values, MethodInfo getter)
        {
            if (colBuff.Length > 0)
            {
                colBuff.Append(@", ");
                valBuff.Append(@", ");
            }

            colBuff.Append("[" + property.ColumName + "]");

            valBuff.Append("{");
            valBuff.Append(index);
            valBuff.Append("}");

            if (property.HasAttribute<ForeignKeyReferenceAttribute>()
                && _dtoMetadataCache.GetMetadataFor(
                    property.GetAttribute<ForeignKeyReferenceAttribute>().ReferencedDto).TableName ==
                operation.OwnerMetadata.TableName)
            {
                values.Add(
                    new Func<object>(() => operation.OwnerPrimaryKey));
                //GetPossiblyUnknownPrimaryKeyValue(operation.OwnerPrimaryKey));
            }
            else if (property.HasAttribute<ManyToOneAttribute>())
            {
                object propValue = property.GetValue(operation.Value);
                DtoMetadata propMetadata = _dtoMetadataCache.GetMetadataFor(property.Prop.PropertyType);
                values.Add(
                    new Func<object>(
                        () =>
                            null == propValue || null == propMetadata
                                ? null
                                : propMetadata.GetPrimaryKeyValue(propValue)));
                //GetPossiblyUnknownPrimaryKeyValue(null == propValue || null == propMetadata ? null : propMetadata.GetPrimaryKeyValue(propValue)));
            }
            else
            {
                values.Add(getter.Invoke(operation.Value, new object[0]));
            }

            ++index;
        }

        private static void FormatWithParm(
            Script script,
            string formatString,
            ref int parmIndex,
            params object [] parmValues)
        {
            var parmNames = new ArrayList();
            foreach (object parmValue in parmValues)
            {
                string parmName = "p" + parmIndex;
                script.Parameters[parmName] = parmValue;
                parmNames.Add("@" + parmName);
                ++parmIndex;
            }

            script.Buffer.Append(string.Format(formatString, parmNames.ToArray()));
        }
    }
}
