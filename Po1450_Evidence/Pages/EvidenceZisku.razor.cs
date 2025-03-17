using Microsoft.JSInterop;
using System.Globalization;

namespace Po1450_Evidence.Pages
{
	public partial class EvidenceZisku
	{
		#region Udalosti formulare
		protected override void OnInitialized()
		{
			base.OnInitialized();
			CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("cs-CZ");
		}
		#endregion

		#region Vlastnosti
		public List<Models.Polozka> Polozky { get; private set; } = new List<Models.Polozka>();
		private Models.Polozka Polozka { get; set; } = new Models.Polozka();

		/// <summary>
		/// Textový výpis výsledků pro různé akce, například počet záznamů nebo statistiky.
		/// </summary>
		public string? Vypis { get; private set; }
		/// <summary>
		/// Počet všech záznamů v seznamu položek.
		/// </summary>
		public int PocetZaznamu => Polozky.Count;
		/// <summary>
		/// Indikuje, zda seznam položek není prázdný.
		/// </summary>
		public bool IsPolozky => Polozky.Count > 0;

		/// <summary>
		/// Indikuje, zda je seznam položek prázdný.
		/// </summary>
		public bool IsNotPolozky => !IsPolozky;
		/// <summary>
		/// Určuje, zda je zapnuto režim editace pro vybranou položku.
		/// </summary>
		public bool IsEditace { get; private set; } = false;
		/// <summary>
		/// Určuje, zda se má zobrazit filtr dat a výsledky filtrování.
		/// </summary>
		public bool ZobrazenFiltrDat { get; set; } = false;
		/// <summary>
		/// Určuje typ filtru pro zisk: &lt;, = nebo &gt;.
		/// </summary>
		public string SelectedFilter { get; set; } = "=";
		/// <summary>
		/// Číselná hodnota, podle které se filtruje zisk položek.
		/// </summary>
		public double FiltrHodnota { get; set; }

		/// <summary>
		/// Textový filtr pro porovnávání s obsahem popisu položek.
		/// </summary>
		public string FiltrPopis { get; set; } = "";

		private List<Models.Polozka> PolozkyFiltr { get; set; } = new List<Models.Polozka>();

		private List<Models.Polozka> PolozkySeskupene => Polozky.OrderBy(x => x.Datum)
			.GroupBy(g=> new { g.Datum.Year, g.Datum.Month})
			.Select(x=> new Models.Polozka(new DateOnly(x.Key.Year, x.Key.Month,1), 
				x.Sum(y=>y.Naklady), x.Sum(y=>y.Vynosy), 
				string.Join(", ",x.Select(y=>y.Popis)))).ToList();
		
		private List<Models.Polozka> PolozkySeskupeneV2 => (from p in Polozky
															group p by new { p.Datum.Year, p.Datum.Month } into g
															orderby g.Key.Year, g.Key.Month
															select new Models.Polozka(new DateOnly(g.Key.Year, g.Key.Month, 1),
																g.Sum(y => y.Naklady), g.Sum(y => y.Vynosy),
																string.Join(", ", g.Select(y => y.Popis)))
															).ToList();
															

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

		
		/// <summary>
		/// Zobrazí všechny položky v textovém řetězci.
		/// </summary>
		public void ZobrazZaznamy()
		{
			Vypis = "Jednotlivé záznamy: <br>" + string.Join("<br>", Polozky);
		}

		/// <summary>
		/// Zobrazí počet všech zaznamenaných položek.
		/// </summary>
		public void ZobrazPocetZaznamu()
		{
			Vypis = $"Počet záznamů: {PocetZaznamu}";
		}

		/// <summary>
		/// Zobrazí statistiky (minimum, maximum a průměr) zisků.
		/// </summary>
		public void Statistiky()
		{
			string vypis = "";
			vypis += "Minimum: " + Minimum().ToString("C2") + "<br>";
			vypis += "Maximum: " + Maximum().ToString("C2") + "<br>";
			vypis += "Průměr: " + Prumer().ToString("C2");
			Vypis = vypis;
		}

		/// <summary>
		/// Vrátí minimální zisk ze všech položek.
		/// </summary>
		/// <returns>Nejmenší hodnota zisku, nebo NaN pokud je seznam prázdný.</returns>
		public double Minimum()
		{
			if (Polozky.Count == 0)
			{
				return double.NaN;
			}
			return Polozky.Min(x => x.Zisk);
		}

		/// <summary>
		/// Vrátí maximální zisk ze všech položek.
		/// </summary>
		/// <returns>Největší hodnota zisku, nebo NaN pokud je seznam prázdný.</returns>
		public double Maximum()
		{
			if (Polozky.Count == 0)
			{
				return double.NaN;
			}
			return Polozky.Max(x => x.Zisk);
		}

		/// <summary>
		/// Vrátí průměrný zisk ze všech položek.
		/// </summary>
		/// <returns>Průměrná hodnota zisku, nebo NaN pokud je seznam prázdný.</returns>
		private double Prumer()
		{
			if (Polozky.Count == 0)
			{
				return double.NaN;
			}
			return Polozky.Average(x => x.Zisk);
		}

		/// <summary>
		/// Seřadí položky vzestupně podle jejich data.
		/// </summary>
		public void Setridit()
		{
			Polozky.Sort((x, y) => x.Datum.CompareTo(y.Datum));
		}

		/// <summary>
		/// Seřadí položky sestupně podle jejich data.
		/// </summary>
		public void SetriditSestupne()
		{
			Polozky.Sort((x, y) => y.Datum.CompareTo(x.Datum));
		}

		/// <summary>
		/// Vrací CSS třídu pro zvýraznění vybrané položky v tabulce, je-li právě editována.
		/// </summary>
		/// <param name="polozka">Položka, pro kterou se vyhodnocuje CSS třída.</param>
		/// <returns>Název CSS třídy, pokud je položka editována; jinak prázdný řetězec.</returns>
		private string GetCssClassForTR(Models.Polozka polozka)
		{
			return polozka == Polozka ? "table-primary" : "";
		}

		/// <summary>
		/// Filtrované položky podle typu filtru zisku a volitelného filtru popisu položky.
		/// </summary>
		public void FiltrujPolozky()
		{
			switch (SelectedFilter)
			{
				case "<":
					PolozkyFiltr = Polozky.Where(x => x.Zisk < FiltrHodnota).ToList();
					break;
				case ">":
					PolozkyFiltr = Polozky.Where(x => x.Zisk > FiltrHodnota).ToList();
					break;
				case "=":
					PolozkyFiltr = Polozky.Where(x => x.Zisk == FiltrHodnota).ToList();
					break;

				default:
					break;
			}

			if (!string.IsNullOrEmpty(FiltrPopis))
			{
				PolozkyFiltr = PolozkyFiltr.Where(x => 
					x.Popis.Contains(FiltrPopis, StringComparison.OrdinalIgnoreCase)).ToList();
			}
			
		}
		#endregion


	}
}
