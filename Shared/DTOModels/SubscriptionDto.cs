﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOModels
{
	public class SubscriptionDto
	{
		public int UserId { get; set; }
		public int BookId { get; set; }
		public DateTime SubscriptionDate { get; set; }
	}
}
