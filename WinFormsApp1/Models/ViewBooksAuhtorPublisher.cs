using System;
using System.Collections.Generic;

namespace WinFormsApp1.Models;

public partial class ViewBooksAuhtorPublisher
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public int? Pages { get; set; }

    public int? Price { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string PublisherName { get; set; } = null!;

    public string Address { get; set; } = null!;
}
