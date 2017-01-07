using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GameStore.Domain.Abstract;
using Moq;
using GameStore.Domain.Entities;
using System.Collections.Generic;
using GameStore.WebUI.Controllers;
using System.Linq;
using System.Web.Mvc;
using GameStore.WebUI.Models;
using GameStore.WebUI.HtmlHelpers;

namespace GameStore.UnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Can_Paginate()
        {
            //arrange - организация
            Mock<IGameRepository> mock = new Mock<IGameRepository>();
            mock.Setup(m => m.Games).Returns(new List<Game>
            {
                new Game {GameId = 1, Name = "Game1" },
                new Game {GameId = 2, Name = "Game2" },
                new Game {GameId = 3, Name = "Game3" },
                new Game {GameId = 4, Name = "Game4" },
                new Game {GameId = 5, Name = "Game5" }
            });
            GameController controller = new GameController(mock.Object);
            controller.pageSize = 3;

            //act - действие
            //IEnumerable<Game> result = (IEnumerable<Game>)controller.List(2).Model;
            GamesListViewModel result = (GamesListViewModel)controller.List(null, 2).Model;
            //assert - утверждение
            List<Game> games = result.Games.ToList();
            Assert.IsTrue(games.Count == 2);
            Assert.AreEqual(games[0].Name, "Game4");
            Assert.AreEqual(games[1].Name, "Game5");
        }

        [TestMethod]
        public void Can_Generate_Page()
        {
            // Организация - определение вспомогательного метода HTML - это необходимо
            // для применения расширяющего метода
            HtmlHelper myHelper = null;

            // Организация - создание объекта PagingInfo
            PagingInfo pagingInfo = new PagingInfo
            {
                CurrentPage = 2,
                TotalItems = 28,
                ItemsPerPage = 10
            };

            // Организация - настройка делегата с помощью лямбда-выражения
            Func<int, string> pageUrlDelegate = i => "Page" + i;

            // Действие
            MvcHtmlString result = myHelper.PageLinks(pagingInfo, pageUrlDelegate);

            // Утверждение
            Assert.AreEqual(@"<a class=""btn btn-default"" href=""Page1"">1</a>"
                + @"<a class=""btn btn-default btn-primary selected"" href=""Page2"">2</a>"
                + @"<a class=""btn btn-default"" href=""Page3"">3</a>",
                result.ToString());
            //<a class="btn btn-default" href="Page1">1</a><a class="btn btn-default btn-primary selected" href="Page2">2</a><a class="btn btn-default" href="Page3">3</a>
        }

        [TestMethod]
        public void Can_Send_Pagination_View_Model()
        {
            Mock<IGameRepository> mock = new Mock<IGameRepository>();
            mock.Setup(m => m.Games).Returns(new List<Game>
            {
                new Game {GameId = 1, Name = "Game1" },
                new Game {GameId = 2, Name = "Game2" },
                new Game {GameId = 3, Name = "Game3" },
                new Game {GameId = 4, Name = "Game4" },
                new Game {GameId = 5, Name = "Game5" }
            });
            GameController controller = new GameController(mock.Object);
            controller.pageSize = 3;

            GamesListViewModel result = (GamesListViewModel)controller.List(null, 2).Model;

            PagingInfo pageInfo = result.PagingInfo;
            Assert.AreEqual(pageInfo.CurrentPage, 2);
            Assert.AreEqual(pageInfo.ItemsPerPage, 3);
            Assert.AreEqual(pageInfo.TotalItems, 5);
            Assert.AreEqual(pageInfo.TotalPages, 2);
        }

        [TestMethod]
        public void Can_Filter_Games()
        {
            Mock<IGameRepository> mock = new Mock<IGameRepository>();
            mock.Setup(m => m.Games).Returns(new List<Game>
            {
                new Game {GameId = 1, Name = "Игра1", Category = "Cat1" },
                new Game { GameId = 2, Name = "Игра2", Category="Cat2"},
                new Game { GameId = 3, Name = "Игра3", Category="Cat1"},
                new Game { GameId = 4, Name = "Игра4", Category="Cat2"},
                new Game { GameId = 5, Name = "Игра5", Category="Cat3"}
            });

            GameController controller = new GameController(mock.Object);
            controller.pageSize = 3;
            List<Game> result = ((GamesListViewModel)controller.List("Cat2", 1).Model).Games.ToList();
            Assert.AreEqual(result.Count(), 2);
            Assert.IsTrue(result[0].Name == "Игра2" && result[0].Category == "Cat2");
            Assert.IsTrue(result[0].Name == "Игра4" && result[0].Category == "Cat2");
        }

        [TestMethod]
        public void Can_Create_Categories()
        {
            Mock<IGameRepository> mock = new Mock<IGameRepository>();
            mock.Setup(m => m.Games).Returns(new List<Game>
            {
                new Game { GameId = 1, Name = "Игра1", Category="Симулятор"},
                new Game { GameId = 2, Name = "Игра2", Category="Симулятор"},
                new Game { GameId = 3, Name = "Игра3", Category="Шутер"},
                new Game { GameId = 4, Name = "Игра4", Category="RPG"},
            });

            NavController target = new NavController(mock.Object);
            List<string> results = ((IEnumerable<string>)target.Menu().Model).ToList();

            Assert.AreEqual(results.Count(), 3);
            Assert.AreEqual(results[0], "RPG");
            Assert.AreEqual(results[1], "Симулятор");
            Assert.AreEqual(results[2], "Шутер");
        }

        [TestMethod]
        public void Indicates_Selected_Category()
        {
            // Организация - создание имитированного хранилища
    Mock<IGameRepository> mock = new Mock<IGameRepository>();
            mock.Setup(m => m.Games).Returns(new Game[] {
        new Game { GameId = 1, Name = "Игра1", Category="Симулятор"},
        new Game { GameId = 2, Name = "Игра2", Category="Шутер"}
    });

            // Организация - создание контроллера
            NavController target = new NavController(mock.Object);

            // Организация - определение выбранной категории
            string categoryToSelect = "Шутер";

            // Действие
            string result = target.Menu(categoryToSelect).ViewBag.SelectedCategory;

            // Утверждение
            Assert.AreEqual(categoryToSelect, result);
        }

        [TestMethod]
        public void Generate_Category_Specific_Game_Count()
        {
            // Организация (arrange)
            Mock<IGameRepository> mock = new Mock<IGameRepository>();
            mock.Setup(m => m.Games).Returns(new List<Game>
    {
        new Game { GameId = 1, Name = "Игра1", Category="Cat1"},
        new Game { GameId = 2, Name = "Игра2", Category="Cat2"},
        new Game { GameId = 3, Name = "Игра3", Category="Cat1"},
        new Game { GameId = 4, Name = "Игра4", Category="Cat2"},
        new Game { GameId = 5, Name = "Игра5", Category="Cat3"}
    });
            GameController controller = new GameController(mock.Object);
            controller.pageSize = 3;

            // Действие - тестирование счетчиков товаров для различных категорий
            int res1 = ((GamesListViewModel)controller.List("Cat1").Model).PagingInfo.TotalItems;
            int res2 = ((GamesListViewModel)controller.List("Cat2").Model).PagingInfo.TotalItems;
            int res3 = ((GamesListViewModel)controller.List("Cat3").Model).PagingInfo.TotalItems;
            int resAll = ((GamesListViewModel)controller.List(null).Model).PagingInfo.TotalItems;

            // Утверждение
            Assert.AreEqual(res1, 2);
            Assert.AreEqual(res2, 2);
            Assert.AreEqual(res3, 1);
            Assert.AreEqual(resAll, 5);
        }
    }
}
