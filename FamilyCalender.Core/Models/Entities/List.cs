namespace FamilyCalender.Core.Models.Entities
{
    public class List
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int OwnerId { get; set; }
        public User? Owner { get; set; }

		public DateTime? CreatedUtc { get; set; }
		public DateTime? LastEditedUtc { get; set; }
       
        public Guid InviteId { get; set; } = Guid.NewGuid();
    }

    public class ListItem
    {
	    public int Id { get; set; }
	    public string Name { get; set; } = string.Empty;

	    public int ListId { get; set; }
	    public List? List { get; set; }
		
	    public int CreatedUserId { get; set; }
	    public User? CreatedUser { get; set; }

	    public DateTime? CreatedUtc { get; set; }
	    public DateTime? LastEditedUtc { get; set; }

	    public ListItemTypeEnum State { get; set; }


		//perhaps to much overhead - maybe delete these
	    public DateTime? StateUpdatedUtc { get; set; }
	    public int UpdateUserId { get; set; }
	    public User? UpdatedUser { get; set; }
	}


	public class ListAccess
    {
	    public int Id { get; set; }
	    public int UserId { get; set; }
	    public User? User { get; set; }
	    public int ListId { get; set; }
	    public List? List { get; set; }
	    public bool IsOwner { get; set; } = false;

        /// <summary>
        /// DateTime when user gained access
        /// </summary>
	    public DateTime? JoinedUtc { get; set; }
    }

	public enum ListTypeEnum
    {
        /// <summary>
        /// Items are being removed when "done". To be used in a Todolist/shopping list
        /// </summary>
        Todolist,

        /// <summary>
        /// Items are being checked when "done". To be used in a Checklist/completion list that all item should have a checkmark at the end
        /// </summary>
        Checklist
    }

	public enum ListItemTypeEnum
	{
		/// <summary>
		/// Not defined, usually when a list is och type TodoList and this item should be deleted on click
		/// </summary>
		Undefined,

		/// <summary>
		/// Used to mark an item as checked
		/// </summary>
		Checked 
	}
}
