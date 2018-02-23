using LogLib;
using NTRCheck.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModelLib;
using NORMLib;

namespace NTRCheck.ViewModels
{
	public abstract class BaseViewModelCollection<ViewModelType,ModelType> : ViewModelCollection<ViewModelType, ModelType>
		where ViewModelType:ViewModel<ModelType>
		where ModelType:new()
	{
		/*public IServer Server
		{
			get;
			private set;
		}*/

		public BaseViewModelCollection(ILogger Logger) : base(Logger)
		{
			//this.Server = Server;
		}

		protected override ModelType OnCreateModel()
		{
			return new ModelType();
		}

		

		protected override bool OnAreModelsAreEquals(ModelType A, ModelType B)
		{
			return ValueType.Equals(Table<ModelType>.PrimaryKey.GetValue(A), Table<ModelType>.PrimaryKey.GetValue(B));
		}

		protected override async Task OnAddModelAsync(ModelType Model, int Index)
		{
			IList<ModelType> source;
			source = (IList<ModelType>)this.Model;
			await Task.Run(() => source.Insert(Index, Model));
		}
		protected override async Task OnRemoveModelAsync(ModelType Model, int Index)
		{
			throw (new NotImplementedException());
		}
		protected override async Task OnEditModelAsync(ModelType Model, int Index)
		{
			throw (new NotImplementedException());
		}

	}
}
