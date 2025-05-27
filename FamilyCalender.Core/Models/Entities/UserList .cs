namespace FamilyCalender.Core.Models.Entities
{
    public class UserList
	{
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int OwnerId { get; set; }
        public User? Owner { get; set; }

		public int SortOrder { get; set; }

		public DateTime? CreatedUtc { get; set; }
		public DateTime? LastEditedUtc { get; set; }
       
        public Guid InviteId { get; set; } = Guid.NewGuid();

		public int? CalendarId { get; set; }
		public Calendar? Calendar { get; set; }

        public List<ListItem> Items { get; set; } = new();

        public ListTypeEnum Type { get; set; }
	}

    public class ListItem
    {
	    public int Id { get; set; }
	    public string Name { get; set; } = string.Empty;

	    public int UserListId { get; set; }
	    public UserList? UserList { get; set; }
		
	    public int CreatedUserId { get; set; }
	    public User? CreatedUser { get; set; }

		public int SortOrder { get; set; } 

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
	    public int UserListId { get; set; }
	    public UserList? UserList { get; set; }
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
        Todolist = 0,

        /// <summary>
        /// Items are being checked when "done". To be used in a Checklist/completion list that all item should have a checkmark at the end
        /// </summary>
        Checklist = 1,

        /// <summary>
        /// Just a textarea to use how you want
        /// </summary>
		Notes = 2
    }

    public enum ListItemTypeEnum
	{
		/// <summary>
		/// Not defined, usually when a list is och type TodoList and this item should be deleted on click
		/// </summary>
		Undefined = 0,

		/// <summary>
		/// Used to mark an item as checked
		/// </summary>
		Checked = 1
	}
}
