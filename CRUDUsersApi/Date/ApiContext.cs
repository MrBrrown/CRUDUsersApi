using System;
using CRUDUsersApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CRUDUsersApi.Date
{
	public class ApiContext : DbContext
	{
		public DbSet<User> Users { get; set; }

		public ApiContext(DbContextOptions<ApiContext> options)
			: base(options){}
	}
}

