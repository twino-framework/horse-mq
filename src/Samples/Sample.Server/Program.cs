﻿using System;
using Horse.Messaging.Data;
using Horse.Messaging.Protocol;
using Horse.Messaging.Server;
using Horse.Messaging.Server.Queues;
using Horse.Messaging.Server.Queues.Delivery;
using Horse.Messaging.Server.Transactions;
using Horse.Server;

namespace Sample.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            HorseRider rider = HorseRiderBuilder.Create()
                .ConfigureQueues(cfg =>
                {
                    cfg.Options.Type = QueueType.Push;
                    cfg.EventHandlers.Add(new QueueEventHandler());

                    cfg.UseMemoryQueues(c =>
                    {
                        c.Options.CommitWhen = CommitWhen.AfterAcknowledge;
                        c.Options.PutBack = PutBackDecision.No;
                    });

                    cfg.UsePersistentQueues(null, c =>
                    {
                        c.Options.Acknowledge = QueueAckDecision.WaitForAcknowledge;
                        c.Options.CommitWhen = CommitWhen.AfterSaved;
                    });
                })
                .ConfigureClients(cfg => { cfg.Handlers.Add(new ClientHandler()); })
                .Build();

            rider.Transaction.CreateContainer("TransactionName",
                TimeSpan.FromSeconds(30),
                new QueueTransactionEndpoint(rider.Queue, "CommitQueue"),
                new QueueTransactionEndpoint(rider.Queue, "RollbackQueue"),
                new QueueTransactionEndpoint(rider.Queue, "TimeoutQueue"));

            HorseServer server = new HorseServer();
            server.UseRider(rider);
            server.Run(9999);
        }
    }
}