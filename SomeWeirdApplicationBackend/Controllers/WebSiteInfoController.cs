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
    public class WebSiteInfoController : ControllerBase
    {
        private readonly WebSiteContext _context;
        private readonly HttpClient _httpClient;

        public WebSiteInfoController(WebSiteContext context)
        {
            _context = context;
            _httpClient = new HttpClient();
        }

        // GET: api/WebSiteInfo
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WebSiteInfo>>> GetWebSites()
        {
            return await _context.WebSites.OrderBy(w => w.Domain).ToListAsync();
        }

        // GET: api/WebSiteInfo/5
        [HttpGet("{id}")]
        public async Task<ActionResult<WebSiteInfo>> GetWebSiteStatistics(int id)
        {
            var webSiteStatistics = await _context.WebSites.FindAsync(id);

            if (webSiteStatistics == null)
            {
                return NotFound();
            }

            return webSiteStatistics;
        }

        // GET: api/WebSiteInfo/referred
        [HttpGet()]
        [Route("/api/referred")]
        public async Task<ActionResult<WebSiteInfo>> GetReferredWebSiteStatistics(string url)
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
                        return sites.OrderBy(s => s.Count).Reverse();
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

                var currentDomain = ExtractDomain(url);
                var currentSite = await _context.WebSites.FirstOrDefaultAsync(w => w.Domain == currentDomain);
                if (currentSite == null)
                {
                    currentSite = new WebSiteInfo
                    {
                        Url = url,
                        Domain = currentDomain,
                        LinkedSites = links.ToList(),
                        Count = 1
                    };
                    _context.Add(currentSite);
                    await _context.SaveChangesAsync();
                }
                else if(currentSite.LinkedSites == null || currentSite.LinkedSites.Count == 0)
                {
                    var linkedSites = links.ToList();
                    linkedSites.RemoveAll(l => l.Domain == currentDomain);
                    currentSite.LinkedSites = linkedSites;

                    await _context.SaveChangesAsync();
                }

                return Ok(links);
            }
            catch(Exception ex)
            {
                throw ex;
            }
            
        }

        // PUT: api/WebSiteInfo/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutWebSiteStatistics(int id, WebSiteInfo webSiteStatistics)
        {
            if (id != webSiteStatistics.Id)
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

        // POST: api/WebSiteInfo
        [HttpPost]
        public async Task<ActionResult<WebSiteInfo>> PostWebSiteStatistics(WebSiteInfo webSiteStatistics)
        {
            _context.WebSites.Add(webSiteStatistics);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetWebSiteStatistics", new { id = webSiteStatistics.Id }, webSiteStatistics);
        }

        // DELETE: api/WebSiteInfo/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<WebSiteInfo>> DeleteWebSiteStatistics(int id)
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

        // DELETE: api/WebSiteInfo/
        [HttpDelete()]
        public async Task<ActionResult<WebSiteInfo>> DeleteAllWebSiteStatistics()
        {
            _context.WebSites.RemoveRange(_context.WebSites);
            await _context.SaveChangesAsync();

            return Ok(null);
        }

        private bool WebSiteStatisticsExists(int id)
        {
            return _context.WebSites.Any(e => e.Id == id);
        }

        private bool ValidateUrl(string url)
        {
            if(String.IsNullOrEmpty(url) ||
                !url.StartsWith("http") ||
                !url.StartsWith("https"))
            {
                return false;
            }
            else if (url.Split('.').Length < 2)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private IEnumerable<WebSiteInfo> GetLinkedDomains(string html)
        {
            List<WebSiteInfo> siteInfo = new List<WebSiteInfo>();
            var links = ExtractLinks(html);

            foreach (var link in links)
            {
                var domain = ExtractDomain(link);
                var site = siteInfo.FirstOrDefault(s => s.Domain == domain);
                if (site != null)
                {
                    site.Count++;
                }
                else
                {
                    siteInfo.Add(new WebSiteInfo
                    {
                        Domain = domain,
                        Url = link,
                        Count = 1
                    });
                }
            }

            return siteInfo;
        }

        private static string ExtractDomain(string link)
        {
            link = link.Replace("http://", "").Replace("https://", "");

            var slashPosition = link.IndexOf("/");
            link = slashPosition > 0 ? link.Substring(0, slashPosition) : link;
            var questionMarkPosition = link.IndexOf("?");
            var domain = questionMarkPosition > 0 ? link.Substring(0, questionMarkPosition) : link;

            return domain;
        }

        private static IEnumerable<string> ExtractLinks(string html)
        {
            IEnumerable<string> list = new List<string>();
            Regex regex = new Regex("(?:href|src)=[\"|']?(.*?)[\"|'|>]+", RegexOptions.Singleline | RegexOptions.CultureInvariant);
            if (regex.IsMatch(html))
            {
                list = regex.Matches(html).Select(m => m.Groups[1].Value);
                list = list.Where(l => l.StartsWith("http")); // http, https
            }

            return list;
        }
    }

    class SiteCountComparer : IEqualityComparer<WebSiteInfo>
    {
        public bool Equals(WebSiteInfo x, WebSiteInfo y)
        {
            return x.Count.Equals(y.Count);
        }

        public int GetHashCode(WebSiteInfo obj)
        {
            return obj.Count.GetHashCode();
        }
    }
}
