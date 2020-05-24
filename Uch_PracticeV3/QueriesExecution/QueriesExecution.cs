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
        public static async Task<EnterParams> ExecuteQuery(Queries queryName, string param = null )
        {
            Uch_PracticeEntities db = new Uch_PracticeEntities();
            List<string> colnames = null;
            List<List<string>> rows = null;
            string sheetName = "Оценки студентов";
            switch (queryName)
            {
                case Queries.Students_Marks:
                    {
                        if (param != null)
                        {
                            int mark = Convert.ToInt32(param);
                            rows =
                                (await (from student in db.Students
                                        join gr in db.Groups on student.GroupId equals gr.Id
                                        join spec in db.Specialties on gr.SpecialtyId equals spec.Id
                                        where student.Result == mark
                                        select new
                                        {
                                            stId = student.Id,
                                            stSName = student.Surname,
                                            stName = student.Name,
                                            stPatro = student.Patronymic,
                                            grName = gr.Naming,
                                            SpecialtyQ = spec.Educational_Program
                                        }).ToListAsync()).ConvertAll((t) => new List<string>()
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

                            sheetName = $"Студенты с оценкой {param}";
                        }
                        else
                        {
                            rows =
                                (await (from student in db.Students
                                        join gr in db.Groups on student.GroupId equals gr.Id
                                        join spec in db.Specialties on gr.SpecialtyId equals spec.Id
                                        join lead in db.Leaders on student.LeaderId equals lead.Id
                                        select new
                                        {
                                            stSName = student.Surname,
                                            stName = student.Name,
                                            stPatro = student.Patronymic,
                                            leadSName = lead.Surname,
                                            leadName = lead.Name,
                                            leadPatro = lead.Patronymic,
                                            grName = gr.Naming,
                                            fileName = student.FileNaming,
                                            fileData = student.FileData,
                                            mark=student.Result

                                        }).ToListAsync()).ConvertAll((t) => new List<string>()
                            {t.stSName + " " + t.stName[0] + "." + t.stPatro[0] + ".", t.leadSName
                            + " " + t.leadName[0] + "." + t.leadPatro[0] + ".", t.grName, t.fileName, t.fileData, 
                                            t.mark.ToString()});
                            
                            colnames = new List<string>()
                            {
                                "Студент",
                                "Руководитель",
                                "Группа",
                                "Название файла",
                                "Файл",
                                "Оценка"
                            };
                        }


                        break;
                    }
            }

            db.Dispose();

            return new EnterParams(sheetName, colnames, rows);
        }
    }
}