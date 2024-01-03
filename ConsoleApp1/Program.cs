using System;
using System.Collections.Generic;

public interface ILeaveApprover
{
    void ApproveLeave(LeaveRequest leaveRequest);
}

public class Manager : ILeaveApprover
{
    public void ApproveLeave(LeaveRequest leaveRequest)
    {
        if (leaveRequest != null && leaveRequest.Status != LeaveStatus.Approved)
        {
            leaveRequest.Status = LeaveStatus.Approved;
            Console.WriteLine($"Leave request with ID {leaveRequest.EmployeeId} approved by Manager.");
        }
        else
        {
            Console.WriteLine($"Unable to approve leave request with ID {leaveRequest.EmployeeId}. Request not found or already approved.");
        }
    }
}

public class LeaveManagementSystem
{
    private List<Employee> employees;
    private List<LeaveRequest> leaveRequests;
    private List<ILeaveApprover> leaveApprovers;

    public LeaveManagementSystem()
    {
        employees = new List<Employee>();
        leaveRequests = new List<LeaveRequest>();
        leaveApprovers = new List<ILeaveApprover> { new Manager() };  
    }

    public void AddEmployee(int employeeId, int leaveBalance)
    {
        Employee newEmployee = new Employee { EmployeeId = employeeId, LeaveBalance = leaveBalance };
        employees.Add(newEmployee);

        Console.WriteLine("Employee added successfully.");
    }

    public void RequestLeave(int employeeId, DateTime startDate, DateTime endDate)
    {
        try
        {
            Employee employee = GetEmployeeById(employeeId);

            if (employee != null)
            {
                int requestedLeaveDays = (int)(endDate - startDate).TotalDays + 1;

                if (employee.LeaveBalance >= requestedLeaveDays)
                {
                    LeaveRequest leaveRequest = new LeaveRequest(employeeId, startDate, endDate);
                    leaveRequests.Add(leaveRequest);

                    Console.WriteLine("Leave request submitted successfully.");
                    employee.LeaveBalance -= requestedLeaveDays;
                }
                else
                {
                    Console.WriteLine("Insufficient leave balance. Leave request not submitted.");
                }
            }
            else
            {
                Console.WriteLine("Employee not found.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    public void ApproveLeave(int requestId)
    {
        try
        {
            LeaveRequest leaveRequest = GetLeaveRequestById(requestId);

            if (leaveRequest != null && leaveRequest.Status != LeaveStatus.Approved)
            {
               
                foreach (var approver in leaveApprovers)
                {
                    approver.ApproveLeave(leaveRequest);
                }

                Employee employee = GetEmployeeById(leaveRequest.EmployeeId);
                int approvedLeaveDays = (int)(leaveRequest.EndDate - leaveRequest.StartDate).TotalDays + 1;
                
            }
            else
            {
                Console.WriteLine($"Unable to approve leave request with ID {requestId}. Request not found or already approved.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    public void DisplayLeaveHistory(int employeeId)
    {
        try
        {
            Employee employee = GetEmployeeById(employeeId);

            if (employee != null)
            {
                Console.WriteLine($"Leave History for Employee {employeeId}:");
                Console.WriteLine($"Leave Balance: {employee.LeaveBalance}");

                Console.WriteLine("Leave Requests:");
                foreach (var request in leaveRequests)
                {
                    if (request.EmployeeId == employeeId)
                    {
                        Console.WriteLine($"Start Date: {request.StartDate}, End Date: {request.EndDate}, Status: {request.Status}");
                    }
                }
            }
            else
            {
                Console.WriteLine("Employee not found.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    private Employee GetEmployeeById(int employeeId)
    {
        foreach (var employee in employees)
        {
            if (employee.EmployeeId == employeeId)
            {
                return employee;
            }
        }
        return null;
    }

    private LeaveRequest GetLeaveRequestById(int requestId)
    {
        foreach (var request in leaveRequests)
        {
            if (request.Status == LeaveStatus.Pending && request.EmployeeId == requestId)
            {
                return request;
            }
        }
        return null;
    }
}

public enum LeaveStatus
{
    Pending,
    Approved
}

public class Employee
{
    public int EmployeeId { get; set; }
    public int LeaveBalance { get; set; }
}

public class LeaveRequest
{
    public int EmployeeId { get; }
    public DateTime StartDate { get; }
    public DateTime EndDate { get; }
    public LeaveStatus Status { get; set; }

    public LeaveRequest(int employeeId, DateTime startDate, DateTime endDate)
    {
        EmployeeId = employeeId;
        StartDate = startDate;
        EndDate = endDate;
        Status = LeaveStatus.Pending;
    }
}

class Program
{
    static void Main()
    {
        LeaveManagementSystem leaveManagementSystem = new LeaveManagementSystem();

        while (true)
        {
            Console.WriteLine("Leave Management System");
            Console.WriteLine("1. Add Employee");
            Console.WriteLine("2. Request Leave");
            Console.WriteLine("3. Display Leave History");
            Console.WriteLine("4. Approve Leave");
            Console.WriteLine("5. Exit");
            Console.Write("Enter your choice: ");

            int choice;
            if (int.TryParse(Console.ReadLine(), out choice))
            {
                switch (choice)
                {
                    case 1:
                        AddEmployeeOperation(leaveManagementSystem);
                        break;
                    case 2:
                        RequestLeaveOperation(leaveManagementSystem);
                        break;
                    case 3:
                        DisplayLeaveHistoryOperation(leaveManagementSystem);
                        break;
                    case 4:
                        ApproveLeaveOperation(leaveManagementSystem);
                        break;
                    case 5:
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Try again.");
                        break;
                }
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a number.");
            }

            Console.WriteLine("\nPress Enter to continue...");
            Console.ReadLine();
            Console.Clear();
        }
    }

    private static void AddEmployeeOperation(LeaveManagementSystem leaveManagementSystem)
    {
        Console.Write("Enter Employee Id: ");
        int employeeId = int.Parse(Console.ReadLine());

        Console.Write("Enter Leave Balance: ");
        int leaveBalance = int.Parse(Console.ReadLine());

        leaveManagementSystem.AddEmployee(employeeId, leaveBalance);
    }

    private static void RequestLeaveOperation(LeaveManagementSystem leaveManagementSystem)
    {
        Console.Write("Enter Employee Id: ");
        int employeeId = int.Parse(Console.ReadLine());

        Console.Write("Enter Start Date (MM/dd/yyyy): ");
        DateTime startDate = DateTime.Parse(Console.ReadLine());

        Console.Write("Enter End Date (MM/dd/yyyy): ");
        DateTime endDate = DateTime.Parse(Console.ReadLine());

        leaveManagementSystem.RequestLeave(employeeId, startDate, endDate);
    }

    private static void DisplayLeaveHistoryOperation(LeaveManagementSystem leaveManagementSystem)
    {
        Console.Write("Enter Employee Id: ");
        int employeeId = int.Parse(Console.ReadLine());

        leaveManagementSystem.DisplayLeaveHistory(employeeId);
    }

    private static void ApproveLeaveOperation(LeaveManagementSystem leaveManagementSystem)
    {
        Console.Write("Enter Leave Request ID to Approve: ");
        int requestId = int.Parse(Console.ReadLine());

        leaveManagementSystem.ApproveLeave(requestId);
    }
}