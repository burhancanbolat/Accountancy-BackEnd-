﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace ZeusApp.Application.Interfaces.Repositories;

public interface IUnitOfWork : IDisposable
{
    Task<int> Commit(CancellationToken cancellationToken);
    Task Rollback();
}
