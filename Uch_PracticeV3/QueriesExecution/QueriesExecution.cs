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
            string sheetName = "";
            switch (queryName)
            {
                case Queries.Students_Marks:
                    {
                        sheetName = "Оценки студентов";
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
                case Queries.GroupMarks:
                    {
                        sheetName = "Средние оценки (по группам)";
                        rows = (await (from student in db.Students
                                       join gr in db.Groups on student.GroupId equals gr.Id
                                       join spec in db.Specialties on gr.SpecialtyId equals spec.Id
                                       select new
                                       {
                                           mark = student.Result,
                                           gr = gr.Naming,
                                           spec = spec.Id,
                                           edu = spec.Educational_Program
                                       } into subquery
                                       group subquery by new { subquery.gr, subquery.spec, subquery.edu } into grouping
                                       orderby grouping.Average(g => g.mark) descending
                                       select new
                                       {
                                           gr = grouping.Key.gr,
                                           spec = grouping.Key.spec,
                                           edu = grouping.Key.edu,
                                           Avg = grouping.Average(g => g.mark)
                                       }).ToListAsync()).ConvertAll((t) => new List<string>()
                                       { t.gr, t.spec, t.edu, t.Avg.ToString() });
                        colnames = new List<string>()
                            {
                                "Группа",
                                "Специальность",
                                "Образовательная программа",
                                "Средняя оценка"
                            };
                        break;
                    }
                case Queries.ContractsTerminations:
                    {
                        var expirationYear = Convert.ToInt32(param);
                        rows = (await (from contract in db.Contracts
                                       join organization in db.Organizations
                                       on contract.OrganizationId equals organization.Id
                                       where contract.Termination_Date.Value.Year < expirationYear
                                       select new
                                       {
                                           OrgId = organization.Id,
                                           OrgName = organization.FullNaming,
                                           enterDate = contract.Enter_Date,
                                           termDate = contract.Termination_Date
                                       }).
                                           ToListAsync()).ConvertAll((t) => new List<string>()
                                       { t.OrgId, t.OrgName, t.enterDate.ToShortDateString(),
                                               t.termDate.Value.ToShortDateString() });
                        colnames = new List<string>()
                            {
                                "ОКПО",
                                "Предприятие",
                                "Дата заключения",
                                "Дата расторжения"
                            };

                        sheetName = "Предприятия, с контрактами до " + param + " года";
                        break;
                    }
                case Queries.LeadersStudents:
                    {
                        sheetName = "Количество студентов в распоряжении руководителей";
                        rows = (await (from student in db.Students
                                       join leader in db.Leaders on student.LeaderId equals leader.Id
                                       join rank in db.Ranks on leader.RankId equals rank.Id
                                       group leader by new { leader.Id, leader.Surname, leader.Name, rank.Naming, 
                                           leader.Patronymic }
                                into grouping
                                       orderby grouping.Count() descending
                                       select new
                                       {
                                           surname = grouping.Key.Surname,
                                           name = grouping.Key.Name,
                                           patronymic = grouping.Key.Patronymic,
                                           rank = grouping.Key.Naming,
                                           StudentsCount = grouping.Count()
                                       }).ToListAsync()).ConvertAll((t) => new List<string>()
                                       { t.surname, t.name, t.patronymic, t.rank, t.StudentsCount.ToString() });
                        colnames = new List<string>()
                            {
                                "Фамилия",
                                "Имя",
                                "Отчество",
                                "Должность",
                                "Количество студентов"
                            };
                        break;
                    }
                case Queries.PopularOrganizations:
                    {
                        sheetName = "Самые популярные предприятия";
                        rows = (await (from student in db.Students
                                    join gr in db.Groups on student.GroupId equals gr.Id
                                    join contract in db.Contracts on student.ContractId equals contract.Id
                                    join org in db.Organizations on contract.OrganizationId equals org.Id
                                    join sector in db.Sectors on org.SectorId equals sector.Id
                                    group org by new { org.Id, org.FullNaming, org.Sector.Naming, grId = gr.Id, grNaming = gr.Naming }
                                    into grouping
                                    select new
                                    {
                                        orgId = grouping.Key.Id,
                                        orgNaming = grouping.Key.FullNaming,
                                        orgSector = grouping.Key.Naming,
                                        grId = grouping.Key.grId,
                                        grNaming = grouping.Key.grNaming,
                                        studentsCount = grouping.Count()
                                    } into grouping where grouping.studentsCount == (from student in db.Students
                                                     join gr in db.Groups on student.GroupId equals gr.Id
                                                     join contract in db.Contracts on student.ContractId equals contract.Id
                                                     join org in db.Organizations on contract.OrganizationId equals org.Id
                                                     where gr.Id == grouping.grId
                                                     group org by new { org.Id, grId = gr.Id }
                                                     into subGrouping
                                                     select new
                                                     {
                                                         orgId = subGrouping.Key.Id,
                                                         grId = subGrouping.Key.grId,
                                                         studentsCount = subGrouping.Count()
                                                     } into gr
                                                     group gr by gr.grId into maxGrouping select maxGrouping.Max(g=>g.studentsCount)).FirstOrDefault()
                                                     select new { grouping.grNaming, grouping.studentsCount, grouping.orgNaming, grouping.orgSector}).ToListAsync()).ConvertAll((t) => new List<string>()
                                       { t.grNaming, t.studentsCount.ToString(), t.orgNaming, t.orgSector});
                        colnames = new List<string>()
                         {
                                "Группа",
                                "Количество студентов",
                                "Предприятие",
                                "Отрасль"
                         };
                        break;
                    }
            }

            db.Dispose();

            return new EnterParams(sheetName, colnames, rows);
        }
    }
}