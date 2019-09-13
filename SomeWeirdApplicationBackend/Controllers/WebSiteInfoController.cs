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
            try
            {
                var all = await _context.WebSites.ToListAsync();
                return Ok(all.Distinct(new SiteDomainComparer()).OrderBy(w => w.Domain).AsEnumerable());
            }
            catch(Exception ex)
            {
                throw ex;
            }
            
        }

        // GET: api/WebSiteInfo/interesting
        [HttpGet]
        [Route("interesting")]
        public async Task<ActionResult<IEnumerable<WebSiteInfo>>> GetInterestingWebSites()
        {
            try
            {
                var all = await _context.WebSites.Where(ws => ws.IsInteresting).ToListAsync();
                return Ok(all.Distinct(new SiteDomainComparer()).OrderBy(w => w.Domain).AsEnumerable());
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        // GET: api/WebSiteInfo/5
        [HttpGet("{id}")]
        public async Task<ActionResult<WebSiteInfo>> GetWebSiteInfo(int id)
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
        public async Task<ActionResult<WebSiteInfo>> GetReferredWebSiteInfo(string url)
        {
            if (!ValidateUrl(url))
                return BadRequest("Url has inappropriate format");

            try
            {
                string error = null;
                var random = new Random();
                var task = _httpClient.GetAsync(url);
                var httpContent = await task.ContinueWith(t =>
                {
                    var response = t.Result;
                    if (response.IsSuccessStatusCode)
                    {
                        return response.Content;
                    }
                    error = response.ReasonPhrase;
                    return null;

                }, TaskContinuationOptions.OnlyOnRanToCompletion);

                if (error != null)
                    return BadRequest(error);

                var html = await httpContent.ReadAsStringAsync();
                var links = GetLinks(html);

                
                if (links.Count() == 0)
                    return NotFound();

                await UpdateLinksForSite(url, links);

                var viewModel = links.GroupBy(ws => ws.Domain).Select(g => new
                {
                    Domain = g.Key,
                    Url = g.First().Url,
                    Count = g.ToList().Count
                });

                return Ok(viewModel);
            }
            catch(Exception ex)
            {
                throw ex;
            }
            
        }

        // PUT: api/WebSiteInfo/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutWebSiteInfo(int id, WebSiteInfo webSiteStatistics)
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
                if (!WebSiteInfoExists(id))
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
        public async Task<ActionResult<WebSiteInfo>> PostWebSiteInfo(WebSiteInfo webSiteStatistics)
        {
            _context.WebSites.Add(webSiteStatistics);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetWebSiteStatistics", new { id = webSiteStatistics.Id }, webSiteStatistics);
        }

        // DELETE: api/WebSiteInfo/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<WebSiteInfo>> DeleteWebSiteInfo(int id)
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
        public async Task<ActionResult<WebSiteInfo>> DeleteAllWebSiteInfo()
        {
            _context.WebSites.RemoveRange(_context.WebSites);
            await _context.SaveChangesAsync();

            return Ok(null);
        }

        private bool WebSiteInfoExists(int id)
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

        private IEnumerable<WebSiteInfo> GetLinks(string html)
        {
            List<WebSiteInfo> siteInfo = new List<WebSiteInfo>();
            var links = ExtractLinks(html);

            foreach (var link in links)
            {
                var domain = ExtractDomain(link);
                siteInfo.Add(new WebSiteInfo
                {
                    Domain = domain,
                    Url = link
                });
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

        private async Task UpdateLinksForSite(string url, IEnumerable<WebSiteInfo> links)
        {
            var currentDomain = ExtractDomain(url);
            var currentSite = await _context.WebSites.Include(site => site.LinkedSites).FirstOrDefaultAsync(w => w.Url == url);

            if (currentSite == null)
            {
                var existingLinks = _context.WebSites.ToList().Except(links, new SiteComparer());
                var siteExistingLinks = existingLinks.Intersect(links);
                var siteNotExistingLnks = links.Except(siteExistingLinks);
                var newLinks = siteExistingLinks.Union(siteNotExistingLnks);

                var isInteresting = IsInteresting(newLinks);

                currentSite = new WebSiteInfo
                {
                    Url = url,
                    Domain = currentDomain,
                    LinkedSites = newLinks.ToList(),
                    IsInteresting = isInteresting
                };
                _context.Add(currentSite);
                await _context.SaveChangesAsync();
            }
            else if (currentSite.LinkedSites == null || currentSite.LinkedSites.Count == 0)
            {
                var existingLinks = _context.WebSites.ToList().Except(links, new SiteComparer());
                var siteExistingLinks = existingLinks.Intersect(links);
                var siteNotExistingLnks = links.Except(siteExistingLinks);
                var newLinks = siteExistingLinks.Union(siteNotExistingLnks).ToList();

                var isInteresting = IsInteresting(newLinks);

                currentSite.LinkedSites = newLinks;
                currentSite.IsInteresting = isInteresting;

                await _context.SaveChangesAsync();
            }
        }

        private bool IsInteresting(IEnumerable<WebSiteInfo> links)
        {
            var repeating = links.GroupBy(ws => ws.Domain).Select(g => new
            {
                Domain = g.Key,
                g.ToList().Count
            });
            var minCountByThree = repeating.Min(r => r.Count) * 3;
            var maxCount = repeating.Max(r => r.Count);
            var totalCount = repeating.Count();
            var isInteresting = totalCount <= 16 && maxCount > minCountByThree;

            return isInteresting;
        }
    }

    class SiteDomainComparer : IEqualityComparer<WebSiteInfo>
    {
        public bool Equals(WebSiteInfo x, WebSiteInfo y)
        {
            return x.Domain.Equals(y.Domain);
        }

        public int GetHashCode(WebSiteInfo obj)
        {
            return obj.Domain.GetHashCode();
        }
    }

    class SiteComparer : IEqualityComparer<WebSiteInfo>
    {
        public bool Equals(WebSiteInfo x, WebSiteInfo y)
        {
            return x.Domain.Equals(y.Domain) && x.Url.Equals(y.Url);
        }

        public int GetHashCode(WebSiteInfo obj)
        {
            return obj.Domain.GetHashCode() ^ obj.Url.GetHashCode();
        }
    }
}
