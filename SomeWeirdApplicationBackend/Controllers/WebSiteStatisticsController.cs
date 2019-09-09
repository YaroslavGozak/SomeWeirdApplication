using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SomeWeirdApplicationBackend.Infrastructure;
using SomeWeirdApplicationBackend.Models.WebCrawler;

namespace SomeWeirdApplicationBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebSiteStatisticsController : ControllerBase
    {
        private readonly WebSiteContext _context;
        private readonly HttpClient _httpClient;

        public WebSiteStatisticsController(WebSiteContext context)
        {
            _context = context;
            _httpClient = new HttpClient();
        }

        // GET: api/WebSiteStatistics
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WebSiteStatistics>>> GetWebSites()
        {
            return await _context.WebSites.ToListAsync();
        }

        // GET: api/WebSiteStatistics/5
        [HttpGet("{id}")]
        public async Task<ActionResult<WebSiteStatistics>> GetWebSiteStatistics(int id)
        {
            var webSiteStatistics = await _context.WebSites.FindAsync(id);

            if (webSiteStatistics == null)
            {
                return NotFound();
            }

            return webSiteStatistics;
        }

        // GET: api/WebSiteStatistics/referred
        [HttpGet()]
        [Route("/api/referred")]
        public async Task<ActionResult<WebSiteStatistics>> GetReferredWebSiteStatistics(string url)
        {
            if (!ValidateUrl(url))
                return BadRequest("Url has inappropriate format");

            try
            {
                string error = null;
                var random = new Random();
                var task = _httpClient.GetStringAsync(url);
                var links = await task.ContinueWith(t =>
                {
                    if (t.IsCompleted)
                    {
                        var html = t.Result;
                        var sites = GetLinkedDomains(html);
                        return sites.OrderBy(s => s.Count).Reverse().Take(random.Next(10, 20));
                    }
                    else
                    {
                        error = t.Exception.Message;
                        return null;
                    }
                });

                if (error != null)
                    return BadRequest(error);
                if (links.Count() == 0)
                    return NotFound();
                return Ok(links);
            }
            catch(Exception ex)
            {
                throw ex;
            }
            
        }

        // PUT: api/WebSiteStatistics/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutWebSiteStatistics(int id, WebSiteStatistics webSiteStatistics)
        {
            if (id != webSiteStatistics.InternalId)
            {
                return BadRequest();
            }

            _context.Entry(webSiteStatistics).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WebSiteStatisticsExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/WebSiteStatistics
        [HttpPost]
        public async Task<ActionResult<WebSiteStatistics>> PostWebSiteStatistics(WebSiteStatistics webSiteStatistics)
        {
            _context.WebSites.Add(webSiteStatistics);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetWebSiteStatistics", new { id = webSiteStatistics.InternalId }, webSiteStatistics);
        }

        // DELETE: api/WebSiteStatistics/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<WebSiteStatistics>> DeleteWebSiteStatistics(int id)
        {
            var webSiteStatistics = await _context.WebSites.FindAsync(id);
            if (webSiteStatistics == null)
            {
                return NotFound();
            }

            _context.WebSites.Remove(webSiteStatistics);
            await _context.SaveChangesAsync();

            return webSiteStatistics;
        }

        private bool WebSiteStatisticsExists(int id)
        {
            return _context.WebSites.Any(e => e.InternalId == id);
        }

        private bool ValidateUrl(string url)
        {
            if(String.IsNullOrEmpty(url) ||
                !url.StartsWith("http") ||
                !url.StartsWith("https") ||
                url.Split('.').Length < 2)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private IEnumerable<WebSiteStatistics> GetLinkedDomains(string html)
        {
            List<WebSiteStatistics> siteStatistics = new List<WebSiteStatistics>();
            IEnumerable<string> domains = ExtractDomains(html);

            foreach (var domain in domains)
            {
                var site = siteStatistics.FirstOrDefault(s => s.Url == domain);
                if (site != null)
                {
                    site.Count++;
                }
                else
                {
                    siteStatistics.Add(new WebSiteStatistics
                    {
                        Url = domain,
                        Count = 1
                    });
                }
            }

            return siteStatistics;
        }

        private static string ExtractDomain(string link)
        {
            var slashPosition = link.IndexOf("/");
            link = slashPosition > 0 ? link.Substring(0, slashPosition) : link;
            var questionMarkPosition = link.IndexOf("?");
            var domain = questionMarkPosition > 0 ? link.Substring(0, questionMarkPosition) : link;
            return domain;
        }

        private static IEnumerable<string> ExtractDomains(string html)
        {
            var pattern = "href=[',\"]http(s *)://.+[',\"]";
            var matches = Regex.Matches(html, pattern);
            var parts = matches.Select(m => m.Value);
            parts = parts.Select(p => p.Replace("href=\"http://", "").Replace("href=\"https://", "").Replace("\"", ""));
            parts.Select(p => { return ExtractDomain(p); });
            return parts;
        }
    }

    class SiteCountComparer : IEqualityComparer<WebSiteStatistics>
    {
        public bool Equals(WebSiteStatistics x, WebSiteStatistics y)
        {
            return x.Count.Equals(y.Count);
        }

        public int GetHashCode(WebSiteStatistics obj)
        {
            return obj.Count.GetHashCode();
        }
    }
}
