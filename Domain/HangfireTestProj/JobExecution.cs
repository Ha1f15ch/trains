using Hangfire;
using Hangfire.Server;
using HangfireTestProj.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HangfireTestProj
{
    public class JobExecution
    {
        public static void StartJobs()
        {
			//Запуск оповещения с приветствием каждые 10 минут на EmaiL с приветствием всех пользователей
			var jobId = "SendEmailJob"; // Уникальный идентификатор задачи
			RecurringJob.AddOrUpdate<EmailSenderWeekDelayJob>(
				jobId,
				job => job.SendEmail(),
				"*/10 * * * *"); // cron выражение для каждых 10 минут
		}
    }
}
