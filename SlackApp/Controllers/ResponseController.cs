using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SlackApp.Data;
using SlackApp.Model;

namespace SlackApp.Controllers
{
    [Route("api/[controller]")]
    public class ResponseController : Controller
    {
        public AppResponseContext _context;

        public ResponseController(AppResponseContext context)
        {
            _context = context;
        }
        // GET api/values
        [HttpGet]
        public ViewResult View()
        {
            return new ViewResult();
        }

        [HttpGet("all")]
        public List<RegexResponseEntry> GetAll()
        {
            return _context.regexResponses.ToList();
        }

        // GET api/values/5
        [HttpGet("{regex}")]
        public RegexResponseEntry Get(string regex)
        {
            return _context.regexResponses.Find(regex);
        }

        // POST api/values
        [HttpPost]
        public StatusCodeResult Post([FromBody]RegexResponseEntry value)
        {
            
            try
            {
                _context.regexResponses.Add(value);
                _context.SaveChanges();
            } catch (Exception e)
            {
                return new StatusCodeResult(500);
            }
            return new StatusCodeResult(200);
        }

        // PUT api/values/5
        [HttpPut]
        public StatusCodeResult Put([FromBody]RegexResponseEntry value)
        {
            try
            {
                _context.regexResponses.Update(value);
                _context.SaveChanges();
            } catch (Exception e)
            {
                return new StatusCodeResult(500);
            }
            return new StatusCodeResult(200);
        }

        // DELETE api/values/5
        [HttpDelete("{regex}")]
        public StatusCodeResult Delete(string regex)
        {
            try
            {
                _context.regexResponses.Remove(_context.Find<RegexResponseEntry>(regex));
                _context.SaveChanges();
            } catch (Exception e)
            {
                return new StatusCodeResult(500);
            }
            return new StatusCodeResult(200);
        }
    }
}
