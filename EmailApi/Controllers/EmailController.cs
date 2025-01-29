using EmailApi;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;

[Route("api/[controller]")]
[ApiController]
public class EmailMessagesController : ControllerBase
{
    private readonly EmailDbContext _context;
    private readonly EmailService _emailService;

    public EmailMessagesController(EmailDbContext context, EmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    // GET: api/EmailMessages
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EmailMessage>>> GetEmailMessages()
    {
        return await _context.EmailMessage.ToListAsync();
    }

    // GET: api/EmailMessages/5
    [HttpGet("{id}")]
    public async Task<ActionResult<EmailMessage>> GetEmailMessage(int id)
    {
        var emailMessage = await _context.EmailMessage.FindAsync(id);
        if (emailMessage == null) return NotFound();
        return emailMessage;
    }

    // POST: api/EmailMessages
    [HttpPost]
    public async Task<ActionResult<EmailMessage>> PostEmailMessage(string toEmail, string subject, string body)
    {
      
        // Send email asynchronously
        var isSent = await _emailService.SendEmailAsync(toEmail, subject, body);
        
        if (isSent)
        {
            var response = new EmailMessage()
            {
                Id = Guid.NewGuid(),
                IsSent = true,
            };

            await _context.SaveChangesAsync();

            return Ok(response);

        }
        else
        {
            return CreatedAtAction(nameof(GetEmailMessage), new { isSent = isSent });
        }


    }

    // PUT: api/EmailMessages/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutEmailMessage(Guid id, EmailMessage emailMessage)
    {
        if (id != emailMessage.Id) return BadRequest();

        _context.Entry(emailMessage).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!EmailMessageExists(id)) return NotFound();
            throw;
        }

        return NoContent();
    }

    // DELETE: api/EmailMessages/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEmailMessage(int id)
    {
        var emailMessage = await _context.EmailMessage.FindAsync(id);
        if (emailMessage == null) return NotFound();

        _context.EmailMessage.Remove(emailMessage);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool EmailMessageExists(Guid id) => _context.EmailMessage.Any(e => e.Id == id);
}
