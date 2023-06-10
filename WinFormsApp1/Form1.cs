using WinFormsApp1.Models;
using System;
using System.Linq;
using System.Windows.Forms;
using WinFormsApp1.Models;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        private FlowLayoutPanel flowLayoutPanel;
        public Form1()
        {
            InitializeComponent();
            InitializeDynamicControls();
            LoadData();
        }

        private void InitializeDynamicControls()
        {
            // Створюємо FlowLayoutPanel
            flowLayoutPanel = new FlowLayoutPanel();
            flowLayoutPanel.Dock = DockStyle.Fill;
            Controls.Add(flowLayoutPanel);

            // Додаємо кнопки
            Button addButton = CreateButton("Додати");
            addButton.Click += AddButton_Click;
            flowLayoutPanel.Controls.Add(addButton);

            Button updateButton = CreateButton("Оновити");
            updateButton.Click += UpdateButton_Click;
            flowLayoutPanel.Controls.Add(updateButton);

            Button deleteButton = CreateButton("Видалити");
            deleteButton.Click += DeleteButton_Click;
            flowLayoutPanel.Controls.Add(deleteButton);

            Button refreshButton = CreateButton("Оновити дані");
            refreshButton.Click += RefreshButton_Click;
            flowLayoutPanel.Controls.Add(refreshButton);


        }

        private Button CreateButton(string buttonText)
        {
            Button button = new Button();
            button.Text = buttonText;
            button.AutoSize = true;
            return button;
        }

        private void LoadData()
        {
            using (LibraryDbContext db = new LibraryDbContext())
            {
                var books = db.Books
                    .OrderBy(x => x.Id)
                    .Select(x => new
                    {
                        x.Id,
                        x.Title,
                        AuthorFName = x.Author.FirstName,
                        AuthorLName = x.Author.LastName,
                        x.Price,
                        x.Pages,
                        x.Publisher.PublisherName,
                        PublisherAddress = x.Publisher.Address

                    })
                    .ToList();

                dataGridView1.DataSource = books;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null) return;
            int c = 0;
            foreach (DataGridViewRow row in dataGridView1.SelectedRows)
            {
                c = (int)row.Cells[0].Value;
            }
            using (var db = new LibraryDbContext())
            {

                var book = (from b in db.Books
                            where b.Id == c
                            select b)
                     .FirstOrDefault();
                if (MessageBox.Show("Видалити запис?", "Підтвердження", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    if (book != null)
                    {
                        db.Books.Remove(book);
                    }
                    else
                    {
                        MessageBox.Show("Обраний запис не знайдено!");
                    }

                }
                db.SaveChanges();
            }
            LoadData();
        }

        private void UpdateButton_Click(object sender, EventArgs e)
        {
            int selectedRowId = (int)dataGridView1.CurrentRow.Cells["Id"].Value;

            using (LibraryDbContext dbContext = new LibraryDbContext())
            {
                Book book = dbContext.Books.FirstOrDefault(b => b.Id == selectedRowId);
                if (book != null)
                {
                    Author author = dbContext.Authors.FirstOrDefault(a => a.Id == book.AuthorId);
                    Publisher publisher = dbContext.Publishers.FirstOrDefault(p => p.Id == book.PublisherId);
                    ObjectHelper form = new ObjectHelper(book.Author.FirstName, book.Author.LastName, book.Publisher.PublisherName, book.Publisher.Address, book.Title, (int)book.Pages, (int)book.Price);

                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        author = dbContext.Authors.FirstOrDefault(a => a.FirstName == form.AuthorFName && a.LastName == form.AuthorLName);
                        if (author == null)
                        {
                            author = new Author
                            {
                                Id = book.AuthorId,
                                FirstName = form.AuthorFName,
                                LastName = form.AuthorLName,
                            };
                            dbContext.Authors.Add(author);
                            dbContext.SaveChanges();
                        }

                        publisher = dbContext.Publishers.FirstOrDefault(p => p.PublisherName == form.PublisherName && p.Address == form.PublisherAddress);
                        if (publisher == null)
                        {
                            publisher = new Publisher
                            {
                                Id = book.PublisherId,
                                PublisherName = form.PublisherName,
                                Address = form.PublisherAddress
                            };
                            dbContext.Publishers.Add(publisher);
                            dbContext.SaveChanges();
                        }

                        book.Title = form.BookTitle;
                        book.AuthorId = author.Id;
                        book.Pages = form.Pages;
                        book.Price = form.Price;
                        book.PublisherId = publisher.Id;

                        dbContext.SaveChanges();
                    }
                }
                else
                {
                    MessageBox.Show("Обраного запису не знайдено.");
                }
            }

            LoadData();
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            ObjectHelper form = new ObjectHelper();
            if (form.ShowDialog() == DialogResult.OK)
            {
                using (LibraryDbContext dbContext = new LibraryDbContext())
                {
                    Author author = dbContext.Authors.FirstOrDefault(a => a.FirstName == form.AuthorFName && a.LastName == form.AuthorLName);
                    if (author == null)
                    {
                        author = new Author
                        {
                            FirstName = form.AuthorFName,
                            LastName = form.AuthorLName,
                        };
                        dbContext.Authors.Add(author);
                        dbContext.SaveChanges();
                    }

                    Publisher publisher = dbContext.Publishers.FirstOrDefault(p => p.PublisherName == form.PublisherName && p.Address == form.PublisherAddress);
                    if (publisher == null)
                    {
                        publisher = new Publisher
                        {
                            PublisherName = form.PublisherName,
                            Address = form.PublisherAddress
                        };
                    }
                    dbContext.Publishers.Add(publisher);
                    dbContext.SaveChanges();

                    Book book = new Book
                    {
                        AuthorId = author.Id,
                        PublisherId = publisher.Id,
                        Title = form.BookTitle,
                        Pages = form.Pages,
                        Price = form.Price
                    };
                    dbContext.Books.Add(book);
                    dbContext.SaveChanges();
                }
            }
            LoadData();
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            LoadData();
        }
    }
}
