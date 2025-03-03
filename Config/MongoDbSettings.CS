namespace AljasAuthApi.Models
{
    public class MongoDbSettings
    {
        public required string ConnectionString { get; set; }
        public required string DatabaseName { get; set; }
      
        public required string EmployeesCollectionName { get; set; }

        public required string SubcontractorCollectionName{get;set;}

        
       
        

    }
}
