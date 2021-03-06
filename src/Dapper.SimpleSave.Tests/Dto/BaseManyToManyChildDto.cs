﻿namespace Dapper.SimpleSave.Tests.Dto
{
    public abstract class BaseManyToManyChildDto : BaseChildDto
    {
        protected BaseManyToManyChildDto()
        {
            //  We don't support INSERTs on many to many child tables so we set a child key here,
            //  otherwise it won't work.
            ChildKey = 1;
        }
    }
}
