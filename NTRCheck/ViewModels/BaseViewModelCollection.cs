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
		public IServer Server
		{
			get;
			private set;
		}

		public BaseViewModelCollection(ILogger Logger,IServer Server) : base(Logger)
		{
			this.Server = Server;
		}

		protected override ModelType OnCreateModel()
		{
			return new ModelType();
		}

		public async Task LoadAsync()
		{
			IEnumerable<ModelType> model;

			try
			{
				 model= Server.Execute<ModelType>(new Select<ModelType>());
			}
			catch(Exception ex)
			{
				Log(ex);
				throw (ex);
			}
			await LoadAsync(model);			
		}

		protected override bool OnAreModelsAreEquals(ModelType A, ModelType B)
		{
			return ValueType.Equals(Table<ModelType>.PrimaryKey.GetValue(A), Table<ModelType>.PrimaryKey.GetValue(B));
		}

		protected override async Task OnAddModelAsync(ModelType Model, int Index)
		{
			await Task.Run( ()=> Server.ExecuteNonQuery(new Insert<ModelType>(Model)) );
		}
		protected override async Task OnRemoveModelAsync(ModelType Model, int Index)
		{
			await Task.Run(() => Server.ExecuteNonQuery(new Delete<ModelType>(Model)));
		}
		protected override async Task OnEditModelAsync(ModelType Model, int Index)
		{
			await Task.Run(() => Server.ExecuteNonQuery(new Update<ModelType>(Model)));
		}

	}
}
