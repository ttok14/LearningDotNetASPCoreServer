//using GameDB;

//namespace LearningServer01.Common
//{
//    public interface IValidationRule
//    {
//        ValidationRes_Entity Validate(ITableService tableService, EntityTable entityData);
//    }

//    public class EntityStructureValidationRule : IValidationRule
//    {
//        public ValidationRes_Entity Validate(ITableService tableService, EntityTable entityData)
//        {
//            bool isValid = entityData != null &&
//                entityData.EntityType == E_EntityType.Structure &&
//                tableService.Container.StructureTable_data.ContainsKey(entityData.DetailTableID);

//            return isValid ? ValidationRes_Entity.Success : ValidationRes_Entity.InvalidEntityStructure;
//        }
//    }

//    public static class GlobalValidator
//    {
//        public static ValidationRes_Entity Validate(
//            ITableService tableService,
//            uint entityTid,
//            params IValidationRule[] rules)
//        {
//            if (tableService == null) return ValidationRes_Entity.InvalidTableService;

//            if (tableService.Container.EntityTable_data.TryGetValue(entityTid, out var entityData) == false)
//                return ValidationRes_Entity.EntityNotExist;

//            foreach (var rule in rules)
//            {
//                var result = rule.Validate(tableService, entityData);
//                if (result != ValidationRes_Entity.Success) return result;
//            }

//            return ValidationRes_Entity.Success;
//        }
//    }
//}
