using Microsoft.JSInterop;

namespace Po1450_Evidence.Pages
{
	public partial class EvidenceZisku
	{
		#region Vlastnosti
		public List<Models.Polozka> Polozky { get; private set; } = new List<Models.Polozka>();
		private Models.Polozka Polozka { get; set; } = new Models.Polozka();
		public bool IsEditace { get; set; } = false;
		#endregion

		#region Metody
		private void Pridat()
		{
			Polozky.Add(new Models.Polozka(Polozka.Datum,Polozka.Naklady, Polozka.Vynosy,Polozka.Popis));
			//Polozka = new Models.Polozka();
		}

		private async Task SmazatZaznam(Models.Polozka mazanaPolozka)
		{
			var zprava = $"Opravdu chcete smazat záznam ze dne {mazanaPolozka.Datum} a se ziskem {mazanaPolozka.Zisk}?";
			bool smazat = await JavaScript.InvokeAsync<bool>("confirm", zprava);
			if (smazat)
				Polozky.Remove(mazanaPolozka);
		}

		private void EditujZaznam(Models.Polozka editovanaPolozka)
		{
			Polozka = editovanaPolozka;
			IsEditace = true;
		}

		private void UkoncitEditaci()
		{
			IsEditace = false;
			Polozka = new Models.Polozka();
		}

		private void Setridit()
		{
			Polozky.Sort((x,y)=> x.Datum.CompareTo(y.Datum));
		}
		private void SetriditSestupne()
		{
			Polozky.Sort((x, y) => y.Datum.CompareTo(x.Datum));
		}
		#endregion


	}
}
