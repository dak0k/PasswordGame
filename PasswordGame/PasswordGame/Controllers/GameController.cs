using Microsoft.AspNetCore.Mvc;
using PasswordGame.Models;

namespace PasswordGame.Controllers
{
	public class GameController : Controller
	{
		private GameViewModel _model = new GameViewModel();
		public IActionResult Index()
		{
			return View(_model);
		}

		/// <summary>
		/// Method GET
		/// </summary>
		/// <param name="password">passowrd</param>
		/// <returns>IActionResult</returns>
		public IActionResult CheckPassword(string password)
		{

			if(string.IsNullOrEmpty(password))
			{
				return View("Index");
			}

			List<string> results = new List<string>();
			string backColor = "primary";

			_model.HtmlResult = string.Empty;

			for (int i = 0; i < _model.LastRule; i++)
			{
				if(i >= _model.Rules.Count)
				{
					_model.IsPasswordComplete=true;
					results.Clear();
					results.Add(
						@$"
					<div style='height:25px;'y
					</div>
					<div class='card alert mt-3 align-self-center mx-auto alert-{backColor}'
						 style='width:fit-content; height: auto; padding:10px; max-width:500px;'>
					<a> Your password created <strong>{password}</strong></a>
					</div>"
						);
					break;
				}
				
				if (_model.Rules[i].Action(password))
				{
					_model.LastRule++;
					backColor= "success";
				}
				else{
					backColor= "danger";
				}
				results.Add(

					@$"
					<div style='height:25px;'y
					</div>
					<div class='card alert mt-3 align-self-center mx-auto alert-{backColor}'
						 style='width:fit-content; height: auto; padding:10px; max-width:500px;'>
					<a><strong>Правило {i + 1}:</strong> {_model.Rules[i].Text}</a>
					</div>"

				);
			}
			results.Reverse();
			_model.HtmlResult = string.Join("", results);
			return View("Index",_model);
		}
	}
}
