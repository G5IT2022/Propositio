﻿using Dapper.Contrib.Extensions;

namespace bacit_dotnet.MVC.Entities
{

    public enum STATUS { PLAN, DO, STUDY, ACT, FINISHED}

    [Table ("Suggestion")]
   public class SuggestionEntity
    {
        public int  suggestion_id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public STATUS status { get; set; }
        public bool isJustDoIt { get; set; }
        public int ownership_emp_id { get; set; }
        public int poster_emp_id   { get; set; }
        public int timestamp_id { get; set; }
      
        public EmployeeEntity owner;
        
        public EmployeeEntity poster;
        
        public List<CommentEntity> comments;
        public List<CategoryEntity> categories;

    }
}
