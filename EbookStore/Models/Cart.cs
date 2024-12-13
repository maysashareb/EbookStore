namespace EbookStore.Models
{
    public class Cart
    {
        public int CartID { get; set; } // Primary Key
        public string UserID { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }

        public ICollection<CartItems> CartItems { get; set; }
    }
}
