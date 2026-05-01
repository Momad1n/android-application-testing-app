// <copyright file="TestRunsController.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Backend.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Net.Mime;
    using System.Threading;
    using System.Threading.Tasks;
    using Backend.Data;
    using Backend.Domain;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Контроллер для управления запусками тестов.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces(MediaTypeNames.Application.Json)] // Указываем, что API всегда возвращает JSON (решает S6965)
    public class TestRunsController : ControllerBase
    {
        private readonly AppDbContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestRunsController"/> class.
        /// </summary>
        /// <param name="context">Контекст базы данных приложения.</param>
        public TestRunsController(AppDbContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Создает новый запуск теста для указанного сценария.
        /// </summary>
        /// <param name="scenarioId">ID тестового сценария из БД.</param>
        /// <param name="cancellationToken">Токен отмены операции.</param>
        /// <returns>Созданный объект запуска теста.</returns>
        [HttpPost("{scenarioId}")]
        [ProducesResponseType(typeof(TestRun), StatusCodes.Status200OK)] // Точный код ответа (решает S6965)
        [ProducesResponseType(StatusCodes.Status400BadRequest)]// Возможная ошибка
        public async Task<IActionResult> CreateTestRun(int scenarioId, CancellationToken cancellationToken)
        {
            var testRun = new TestRun
            {
                TestScenarioId = scenarioId,
                CreatedDate = DateTime.UtcNow,
                Status = TestRunStatus.Pending,
            };

            this.context.TestRuns.Add(testRun);
            await this.context.SaveChangesAsync(cancellationToken);

            return this.Ok(testRun);
        }

        /// <summary>
        /// Получает список всех запусков тестов.
        /// </summary>
        /// <param name="cancellationToken">Токен отмены операции.</param>
        /// <returns>Список объектов TestRun.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TestRun>), StatusCodes.Status200OK)] // Точный код ответа (решает S6965)
        public async Task<IActionResult> GetTestRuns(CancellationToken cancellationToken)
        {
            var runs = await this.context.TestRuns.ToListAsync(cancellationToken);
            return this.Ok(runs);
        }
    }
}