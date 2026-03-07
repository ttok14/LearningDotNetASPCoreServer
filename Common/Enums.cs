using System;

namespace LearningServer01
{
    public enum ValidationRes_Entity
    {
        None = 0,

        Success,

        InvalidTableService,
        EntityNotExist,
        InvalidEntityStructure,
    }

    public enum ValidationContent
    {
        None = 0,

        EntityIsStructure,
    }
}
