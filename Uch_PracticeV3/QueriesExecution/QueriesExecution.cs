using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI.WebControls;
using Uch_PracticeV3.Models;
using Uch_PracticeV3.OfficeInteraction;

namespace Uch_PracticeV3.QueriesExecution
{
    public class QueriesExecution
    {
        public static async Task<EnterParams> ExecuteQuery(Queries queryName)
        {
            UCH_PracticeEntities db = new UCH_PracticeEntities();
            List<string> colnames = null;
            List<List<string>> rows = null;
            string sheetName = "Оценки студентов";
            switch (queryName)
            {
                case Queries.Students_Marks:
                    {
                        rows =
                                (await (from student in db.Students
                                join gr in db.Groups on student.GroupId equals gr.Id
                                join spec in db.Specialties on gr.SpecialtyId equals spec.Id
                                select new
                                {
                                    stId = student.Id,
                                    stSName = student.Surname,
                                    stName = student.Name,
                                    stPatro = student.Patronymic,
                                    grName = gr.Naming,
                                    SpecialtyQ = spec.Educational_Program
                                }).ToListAsync()).ConvertAll((t)=>new List<string>() 
                                {t.stId, t.stSName,t.stName, t.stPatro, t.grName, t.SpecialtyQ });

                        colnames = new List<string>()
                        {
                            "Код студента",
                            "Фамилия",
                            "Имя",
                            "Отчество",
                            "Группа",
                            "Направление подготовки"
                        };

                        break;
                    }
            }



            db.Dispose();

            return new EnterParams(sheetName, colnames, rows);
        }
    }
}