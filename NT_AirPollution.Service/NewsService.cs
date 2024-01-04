using Dapper.Contrib.Extensions;
using Newtonsoft.Json;
using NT_AirPollution.Model.Domain;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NT_AirPollution.Service
{
    public class NewsService : BaseService
    {
        public List<News> GetNews()
        {
            using (var cn = new SqlConnection(connStr))
            {
                var news = cn.GetAll<News>().Where(o => o.DeleteDate == null).ToList();
                return news;
            }
        }

        public News GetNews(long id)
        {
            using (var cn = new SqlConnection(connStr))
            {
                var news = cn.Get<News>(id);
                return news;
            }
        }

        public long AddNews(News news)
        {
            using (var cn = new SqlConnection(connStr))
            {
                try
                {
                    long id = cn.Insert(news);
                    return id;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public bool UpdateNews(News news)
        {
            using (var cn = new SqlConnection(connStr))
            {
                try
                {
                    cn.Update(news);
                    return true;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
    }
}
