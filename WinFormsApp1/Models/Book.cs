using System;
using System.Collections.Generic;

namespace WinFormsApp1.Models;

public partial class Book
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public int AuthorId { get; set; }

    public int? Pages { get; set; }

    public int? Price { get; set; }

    public int PublisherId { get; set; }

    public virtual Author Author { get; set; } = null!;

    public virtual Publisher Publisher { get; set; } = null!;
}
