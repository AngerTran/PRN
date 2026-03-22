using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CapstoneReviewTool.Entities;

namespace CapstoneReviewTool.Data
{
    public static class DbSeeder
    {
        private const string DefaultPassword = "Password123!";

        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // 1. Roles
            string[] roleNames = { "Admin", "Committee", "Student" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // 2a. 4 tài khoản Committee (giảng viên / hội đồng)
            var committeeSeeds = new (string Email, string FullName, string LecturerCode)[]
            {
                ("committee@fpt.edu.vn", "Hội đồng — Trần Minh Đức", "COMM01"),
                ("committee02@fpt.edu.vn", "TS. Lê Hoàng Nam", "COMM02"),
                ("committee03@fpt.edu.vn", "ThS. Phạm Thu Hà", "COMM03"),
                ("committee04@fpt.edu.vn", "GV. Đỗ Quang Vinh", "COMM04")
            };

            foreach (var (email, fullName, lecturerCode) in committeeSeeds)
            {
                var user = await userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    user = new ApplicationUser
                    {
                        UserName = email,
                        Email = email,
                        FullName = fullName,
                        EmailConfirmed = true,
                        Role = UserRole.Lecturer,
                        LecturerCode = lecturerCode
                    };
                    await userManager.CreateAsync(user, DefaultPassword);
                    await userManager.AddToRoleAsync(user, "Committee");
                }
                else
                {
                    if (!await userManager.IsInRoleAsync(user, "Committee"))
                    {
                        await userManager.AddToRoleAsync(user, "Committee");
                    }

                    if (string.IsNullOrWhiteSpace(user.LecturerCode))
                    {
                        user.LecturerCode = lecturerCode;
                        await userManager.UpdateAsync(user);
                    }
                }
            }

            // 2b. 20 sinh viên (student01 … student20)
            for (var i = 1; i <= 20; i++)
            {
                var email = $"student{i:00}@fpt.edu.vn";
                var user = await userManager.FindByEmailAsync(email);
                if (user != null)
                {
                    continue;
                }

                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    FullName = $"Sinh viên {i:00}",
                    EmailConfirmed = true,
                    Role = UserRole.Student,
                    StudentId = $"SE25{i:000}"
                };
                await userManager.CreateAsync(user, DefaultPassword);
                await userManager.AddToRoleAsync(user, "Student");
            }

            // Academic term
            if (!await context.AcademicTerms.AnyAsync())
            {
                context.AcademicTerms.Add(new AcademicTerm
                {
                    Code = "SPRING2026",
                    Name = "Spring 2026 — PRN212",
                    IsActive = true
                });
                await context.SaveChangesAsync();
            }

            // 3. Committees + slots (chỉ khi chưa có hội đồng)
            if (!await context.Committees.AnyAsync())
            {
                var committee = new Committee { Name = "Hội đồng Kỹ thuật Phần mềm 1" };
                context.Committees.Add(committee);
                await context.SaveChangesAsync();

                foreach (var (email, _, _) in committeeSeeds)
                {
                    var u = await userManager.FindByEmailAsync(email);
                    if (u != null)
                    {
                        context.CommitteeMembers.Add(new CommitteeMember { CommitteeId = committee.Id, UserId = u.Id });
                    }
                }

                await context.SaveChangesAsync();

                var slots = new List<DefenseSlot>
                {
                    new DefenseSlot
                    {
                        StartTime = DateTime.Now.AddDays(2).Date + new TimeSpan(8, 0, 0),
                        EndTime = DateTime.Now.AddDays(2).Date + new TimeSpan(10, 0, 0),
                        Room = "201",
                        Mode = "Offline",
                        MaxGroups = 3,
                        CommitteeId = committee.Id
                    },
                    new DefenseSlot
                    {
                        StartTime = DateTime.Now.AddDays(2).Date + new TimeSpan(13, 30, 0),
                        EndTime = DateTime.Now.AddDays(2).Date + new TimeSpan(15, 30, 0),
                        Room = "Zoom 1",
                        Mode = "Online",
                        MaxGroups = 3,
                        CommitteeId = committee.Id
                    }
                };
                context.DefenseSlots.AddRange(slots);
                await context.SaveChangesAsync();
            }
            else
            {
                // DB cũ: đảm bảo đủ 4 thành viên trong hội đồng đầu tiên
                var firstCommittee = await context.Committees.OrderBy(c => c.Id).FirstAsync();
                foreach (var (email, _, _) in committeeSeeds)
                {
                    var u = await userManager.FindByEmailAsync(email);
                    if (u == null)
                    {
                        continue;
                    }

                    var exists = await context.CommitteeMembers
                        .AnyAsync(m => m.CommitteeId == firstCommittee.Id && m.UserId == u.Id);
                    if (!exists)
                    {
                        context.CommitteeMembers.Add(new CommitteeMember
                        {
                            CommitteeId = firstCommittee.Id,
                            UserId = u.Id
                        });
                    }
                }

                await context.SaveChangesAsync();
            }
        }
    }
}
