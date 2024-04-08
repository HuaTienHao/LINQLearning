using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LINQLearning
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Get data
            List<Employee> employees = Data.GetEmployees();
            List<Department> departments = Data.GetDepartments();

            UnionEmployee(employees);

            Console.ReadKey();
        }


        public static void QuerySyntax(List<Employee> employees, List<Department> departments)
        {
            //--- LINQ Query Syntax ---
            var querySyntax = from employee in employees
                              join department in departments
                              on employee.DepartmentId equals department.Id
                              select new
                              {
                                  FirstName = employee.FirstName,
                                  LastName = employee.LastName,
                                  AnnualSalary = employee.AnnualSalary,
                                  Manager = employee.IsManager,
                                  Department = department.LongName,
                              };

            var getManager = querySyntax.Where(e => e.Manager).OrderByDescending(e => e.FirstName).ToList();

            foreach (var employee in querySyntax)
            {
                Console.WriteLine($"Name: {employee.FirstName} {employee.LastName}");
                Console.WriteLine($"Annual Salary: {employee.AnnualSalary}");
                Console.WriteLine($"Is manager: {employee.Manager}");
                Console.WriteLine($"Department: {employee.Department}");
                Console.WriteLine();
            }
        }

        public static void MethodSyntax(List<Employee> employees, List<Department> departments, bool innerjoin)
        {
            //--- LINQ Method Syntax ---
            // Inner Join
            if (innerjoin)
            {
                var methodSyntax = departments.Join(
                    employees, department => department.Id,
                    employee => employee.DepartmentId,
                    (department, employee) => new
                    {
                        FullName = employee.FirstName + " " + employee.LastName,
                        AnnualSalary = employee.AnnualSalary,
                        Manager = employee.IsManager,
                        DepartmentName = department.LongName
                    }
                );

                foreach (var employee in methodSyntax)
                {
                    Console.WriteLine($"Name: {employee.FullName}");
                    Console.WriteLine($"Annual Salary: {employee.AnnualSalary}");
                    Console.WriteLine($"Is manager: {employee.Manager}");
                    Console.WriteLine($"Department: {employee.DepartmentName}");
                    Console.WriteLine();
                }
            }
            else
            {
                // Left Join
                /*var leftJoin = departments.GroupJoin(
                        employees,
                        department => department.Id,
                        employee => employee.DepartmentId,
                        (department, employeeGroup) => new
                        {
                            DepartmentName = department.LongName,
                            Employees = employeeGroup
                        }
                    );

                foreach (var item in leftJoin)
                {
                    Console.WriteLine($"Department Name: {item.DepartmentName}");
                    foreach (var employee in item.Employees)
                    {
                        Console.WriteLine($"\t{employee.FirstName} {employee.LastName}");
                    }
                    Console.WriteLine();
                }*/

                var queryLeftJoin = from emp in employees
                                    join dept in departments
                                    on emp.DepartmentId equals dept.Id into employeeGroup
                                    from subgroup in employeeGroup.DefaultIfEmpty()
                                    select new
                                    {
                                        EmployeeName = emp.FirstName + " " + emp.LastName,
                                        DepartmentName = subgroup?.LongName ?? string.Empty,
                                    };

                foreach (var item in queryLeftJoin)
                {
                    Console.WriteLine($"Name: {item.EmployeeName} - Department: {item.DepartmentName}");
                    Console.WriteLine();
                }
            }
        }

        public static void EmployeesOrderByDepartment(List<Employee> employees, List<Department> departments)
        {
            var results = from emp in employees
                          join dept in departments
                          on emp.DepartmentId equals dept.Id
                          orderby emp.DepartmentId descending
                          select new
                          {
                              DepartmentId = emp.DepartmentId,
                              DepartmentName = dept.LongName,
                              FullName = emp.FirstName + " " + emp.LastName,
                              AnnualSalary = emp.AnnualSalary,
                              Manager = emp.IsManager
                          };

            foreach (var item in results)
            {
                Console.WriteLine($"Department Id: {item.DepartmentId}");
                Console.WriteLine($"Department Name: {item.DepartmentName}");
                Console.WriteLine($"Name: {item.FullName}");
                Console.WriteLine($"Annual Salary: {item.AnnualSalary}");
                Console.WriteLine($"Is manager: {item.Manager}");
                Console.WriteLine();
            }
        }

        public static void EmployeesGroupByDepartment(List<Employee> employees, List<Department> departments)
        {
            var results = from emp in employees
                          group emp by emp.DepartmentId;
            foreach (var item in results)
            {
                Console.WriteLine($"Department Id: {item.Key}");
                foreach (Employee employee in item)
                {
                    Console.WriteLine($"\tName: {employee.FirstName} {employee.LastName}");
                }
                Console.WriteLine();
            }
        }

        public static void EmployeeFirstOrDefaultById(List<Employee> employees, int employeeId)
        {
            var emp = employees.FirstOrDefault(e => e.Id == employeeId);

            Console.WriteLine($"Employees Id: {emp.Id}");
            Console.WriteLine($"Name: {emp.FirstName} {emp.LastName}");
        }

        public static void AverageAnualSalaryByDepartmentId(List<Employee> employees, List<Department> departments, int departmentId)
        {
            decimal results = employees.Where(e => e.DepartmentId == departmentId).Average(e => e.AnnualSalary);
            string departmentName = (from dept in departments
                                 where dept.Id == departmentId
                                 select dept.LongName).FirstOrDefault();

            Console.WriteLine($"Average anual salary at department {departmentName}: {results}");
        }

        public static void CountEmployeeByDepartmentId(List<Employee> employees, List<Department> departments, int departmentId)
        {
            int result = employees.Count(e => e.DepartmentId == departmentId);
            string departmentName = (from dept in departments
                                     where dept.Id == departmentId
                                     select dept.LongName).FirstOrDefault();

            Console.WriteLine($"Count of {departmentName}: {result}");

        }

        public static void DistinctEmployee(List<Employee> employees)
        {
            List<Employee> newEmployeeList = employees;
            newEmployeeList.Add(new Employee()
            {
                Id = 5,
                FirstName = "Hao",
                LastName = "Hua",
                AnnualSalary = 30000.2m,
                IsManager = false,
                DepartmentId = 0
            });

            Console.WriteLine("\tBefore Distinct");
            Console.WriteLine("-----------------");
            foreach (Employee employee in newEmployeeList)
            {
                Console.WriteLine($"Name: {employee.FirstName} {employee.LastName}");
                Console.WriteLine($"Annual Salary: {employee.AnnualSalary}");
                Console.WriteLine();
            }

            Console.WriteLine("\tAfter Distinct");
            Console.WriteLine("-----------------");
            foreach (Employee employee in newEmployeeList.Distinct(new EmployeeComparer()))
            {
                Console.WriteLine($"Name: {employee.FirstName} {employee.LastName}");
                Console.WriteLine($"Annual Salary: {employee.AnnualSalary}");
                Console.WriteLine();
            }
        }

        public static void ExceptEmployee(List<Employee> employees)
        {
            Employee except = new Employee()
            {
                Id = 5,
                FirstName = "Hao",
                LastName = "Hua",
                AnnualSalary = 30000.2m,
                IsManager = false,
                DepartmentId = 0
            };
            List<Employee> exceptEmployees = new List<Employee>();
            exceptEmployees.Add(except);

            var afterExcept = employees.Except(exceptEmployees, new EmployeeComparer());

            foreach (Employee employee in afterExcept)
            {
                Console.WriteLine($"Name: {employee.FirstName} {employee.LastName}");
                Console.WriteLine($"Annual Salary: {employee.AnnualSalary}");
                Console.WriteLine();
            }
        }

        public static void IntersectEmployee(List<Employee> employees)
        {
            Employee intersect = new Employee()
            {
                Id = 5,
                FirstName = "Hao",
                LastName = "Hua",
                AnnualSalary = 30000.2m,
                IsManager = false,
                DepartmentId = 0
            };
            List<Employee> intersectEmployees = new List<Employee>();
            intersectEmployees.Add(intersect);

            var afterExcept = employees.Intersect(intersectEmployees, new EmployeeComparer());

            foreach (Employee employee in afterExcept)
            {
                Console.WriteLine($"Name: {employee.FirstName} {employee.LastName}");
                Console.WriteLine($"Annual Salary: {employee.AnnualSalary}");
                Console.WriteLine();
            }
        }

        public static void UnionEmployee(List<Employee> employees)
        {
            Employee employee1 = new Employee()
            {
                Id = 5,
                FirstName = "Hao",
                LastName = "Hua",
                AnnualSalary = 30000.2m,
                IsManager = false,
                DepartmentId = 0
            };
            Employee employee2 = new Employee()
            {
                Id = 6,
                FirstName = "Holy",
                LastName = "Cow",
                AnnualSalary = 30000.2m,
                IsManager = false,
                DepartmentId = 0
            };

            List<Employee> list2 = new List<Employee>();
            list2.Add(employee1);
            list2.Add(employee2);

            var unionEmployee = employees.Union(list2, new EmployeeComparer());
            foreach (Employee employee in unionEmployee)
            {
                Console.WriteLine($"Name: {employee.FirstName} {employee.LastName}");
                Console.WriteLine($"Annual Salary: {employee.AnnualSalary}");
                Console.WriteLine();
            }
        }

        /*public static void AggregateEmployeeAnualSalary(List<Employee> employees)
        {
            decimal totalSalary = employees.Aggregate<Employee, decimal>(0, (totalSalary, e) =>
            {
                decimal bonus = (e.IsManager) ? 0.04m : 0.02m;

                totalSalary = totalSalary + (e.AnnualSalary + (e.AnnualSalary * bonus));
                return totalSalary;
            });

            Console.WriteLine(totalSalary);
        }*/
    }
}
