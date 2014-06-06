using System;
using System.Web.Mvc;

namespace NRedisApi.Samples.Mvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRedisConnectionFactory _connectionFactory;
        private int _i;
        private const string UrnPrefix = "urn:MvcTest";

        public HomeController(IRedisConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
            _i = 1;
                //_connectionFactory.GetConnection().RedisString().As<SomethingToStore>().Urn(string.Format(@"{0}{1}", UrnPrefix, _i)).Get().Id++;
        }

        public ActionResult Index()
        {
            var something = new SomethingToStore(_i, "Hello");

            var command = _connectionFactory.GetConnection();

            command
                .SetUrn(UrnPrefix)
                .RedisString()
                .AsType<SomethingToStore>()
                .Set(something);

            var model = _connectionFactory.GetConnection().RedisString().AsType<SomethingToStore>().SetUrn(UrnPrefix).Get();

            return View(model);
        }

        [HttpPost]
        public ActionResult Index(string q)
        {
            ++_i;
            var something = new SomethingToStore(_i, q);

            var command = _connectionFactory.GetConnection();

            command
                .SetUrn(string.Format(@"{0}{1}", UrnPrefix, _i))
                .RedisString()
                .AsType<SomethingToStore>()
                .Set(something);

            var model = _connectionFactory.GetConnection().RedisString().AsType<SomethingToStore>().SetUrn(string.Format(@"{0}{1}", UrnPrefix, _i)).Get();

            return View(model);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public class SomethingToStore
        {
            public SomethingToStore(int id, string name)
            {
                Id = id;
                Name = name;
                Created = DateTime.Now;
            }

            public int Id { get; set; }
            public string Name { get; set; }
            public DateTime Created { get; set; }
        }
    }
}