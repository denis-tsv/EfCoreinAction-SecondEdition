﻿// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using ServiceLayer.CheckoutServices.Concrete;
using ServiceLayer.OrderServices.Concrete;
using Test.Mocks;
using Test.TestHelpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestServiceLayer
{
    public class Ch04_DisplayOrderService
    {
        [Fact]
        public void TestGetOrderDetailNotFound()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                var service = new DisplayOrdersService(context);

                //ATTEMPT
                var ex = Assert.Throws<NullReferenceException>(() => service.GetOrderDetail(1));

                //VERIFY
                ex.Message.ShouldEqual("Could not find the order with id of 1.");
            }
        }


        [Fact]
        public void TestGetOrderDetailOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();
                var userId = Guid.NewGuid();

                var order = new Order
                {
                    CustomerName = userId,
                    LineItems = new List<LineItem>
                    {
                        new LineItem
                        {
                            BookId = 1,
                            LineNum = 0,
                            BookPrice = 123,
                            NumBooks = 456
                        }
                    }
                };
                context.Orders.Add(order);
                context.SaveChanges();
                var service = new DisplayOrdersService(context);

                //ATTEMPT
                var dto = service.GetOrderDetail(1);

                //VERIFY
                var lineItems = dto.LineItems.ToList();
                lineItems.Count.ShouldEqual(1);
                lineItems.First().BookId.ShouldEqual(1);
                lineItems.First().BookPrice.ShouldEqual(123);
                lineItems.First().NumBooks.ShouldEqual((short)456);
            }
        }

        [Fact]
        public void TestGetUsersOrdersOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();
                var userId = Guid.NewGuid();

                var order = new Order
                {
                    CustomerName = userId,
                    LineItems = new List<LineItem>
                    {
                        new LineItem
                        {
                            BookId = 1,
                            LineNum = 0,
                            BookPrice = 123,
                            NumBooks = 456
                        }
                    }
                };
                context.Orders.Add(order);
                context.SaveChanges();
                var mockCookieRequests = new MockHttpCookieAccess(BasketCookie.BasketCookieName, $"{userId}");
                var service = new DisplayOrdersService(context);

                //ATTEMPT
                var orders = service.GetUsersOrders(mockCookieRequests.CookiesIn);

                //VERIFY
                orders.Count.ShouldEqual(1);
                orders.First().LineItems.ShouldNotBeNull();
                var lineItems = orders.First().LineItems.ToList();
                lineItems.Count.ShouldEqual(1);
                lineItems.First().BookId.ShouldEqual(1);
                lineItems.First().BookPrice.ShouldEqual(123);
                lineItems.First().NumBooks.ShouldEqual((short)456);
            }
        }
    }
}