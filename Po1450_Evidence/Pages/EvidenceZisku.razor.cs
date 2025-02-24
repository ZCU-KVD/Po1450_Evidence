namespace Po1450_Evidence.Pages
{
	public partial class EvidenceZisku
	{
		#region Vlastnosti
		public List<Models.Polozka> Polozky { get; private set; } = new List<Models.Polozka>();
		private Models.Polozka Polozka { get; set; } = new Models.Polozka();
		#endregion
	}
}
