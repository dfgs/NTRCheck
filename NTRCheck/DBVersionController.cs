using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NORMLib;
using NORMLib.VersionControl;
using NTRCheck.Models;

namespace NTRCheck
{
	public class DBVersionController : VersionController
	{
		public DBVersionController(IServer Server) : base(Server, "NTRCheck")
		{
		}

		[Revision(0)]
		public static IEnumerable<IQuery> UpgradeToRevision0()
		{
			yield return new CreateTable<Case>(Case.CaseIDColumn, Case.DescriptionColumn, Case.CoreHostNameColumn, Case.MySqlLoginColumn, Case.MySqlPasswordColumn);
			yield return new CreateTable<CVS>(CVS.CVSIDColumn, CVS.CaseIDColumn, CVS.CVSKEYColumn, CVS.CVSUSRColumn, CVS.CVSCHNColumn, CVS.UtcStartTimeColumn, CVS.UtcEndTimeColumn, CVS.CVSSDTColumn, CVS.CVSEDTColumn, CVS.CVSTYPColumn, CVS.CVSC05Column);

		}
	
		[Revision(1)]
		public static IEnumerable<IQuery> UpgradeToRevision1()
		{
			yield return new CreateRelation<Case, CVS, int?>(Case.CaseIDColumn, CVS.CaseIDColumn);
		
		}
	/*
	[Revision(2)]
	public static IEnumerable<IQuery> UpgradeToRevision2()
	{

		yield return new Insert<ResourceType>(new ResourceType() { Name = "Wood", Size = 1 }, ResourceType.NameColumn, ResourceType.SizeColumn);
		yield return new Insert<ResourceType>(new ResourceType() { Name = "Plank", Size = 1 }, ResourceType.NameColumn, ResourceType.SizeColumn);
		yield return new Insert<ResourceType>(new ResourceType() { Name = "Stone", Size = 1 }, ResourceType.NameColumn, ResourceType.SizeColumn);
		yield return new Insert<ResourceType>(new ResourceType() { Name = "Iron", Size = 1 }, ResourceType.NameColumn, ResourceType.SizeColumn);
		yield return new Insert<ResourceType>(new ResourceType() { Name = "Gold", Size = 1 }, ResourceType.NameColumn, ResourceType.SizeColumn);

		yield return new Insert<FactoryType>(new FactoryType() { Name = "Lumberjack", BuildDuration = 2 }, FactoryType.NameColumn, FactoryType.BuildDurationColumn);
		yield return new Insert<FactoryType>(new FactoryType() { Name = "Sawmill", BuildDuration = 2 }, FactoryType.NameColumn, FactoryType.BuildDurationColumn);
		yield return new Insert<FactoryType>(new FactoryType() { Name = "Stone", BuildDuration = 2 }, FactoryType.NameColumn, FactoryType.BuildDurationColumn);
		yield return new Insert<FactoryType>(new FactoryType() { Name = "Iron", BuildDuration = 2 }, FactoryType.NameColumn, FactoryType.BuildDurationColumn);
		yield return new Insert<FactoryType>(new FactoryType() { Name = "Gold", BuildDuration = 2 }, FactoryType.NameColumn, FactoryType.BuildDurationColumn);

		yield return new Insert<Product>(new Product() { FactoryTypeID = 1, ResourceTypeID = 1, Rate = 1f }, Product.FactoryTypeIDColumn, Product.ResourceTypeIDColumn, Product.RateColumn);
		yield return new Insert<Product>(new Product() { FactoryTypeID = 2, ResourceTypeID = 2, Rate = 1f }, Product.FactoryTypeIDColumn, Product.ResourceTypeIDColumn, Product.RateColumn);
		yield return new Insert<Product>(new Product() { FactoryTypeID = 3, ResourceTypeID = 3, Rate = 1f }, Product.FactoryTypeIDColumn, Product.ResourceTypeIDColumn, Product.RateColumn);
		yield return new Insert<Product>(new Product() { FactoryTypeID = 4, ResourceTypeID = 4, Rate = 1f }, Product.FactoryTypeIDColumn, Product.ResourceTypeIDColumn, Product.RateColumn);
		yield return new Insert<Product>(new Product() { FactoryTypeID = 5, ResourceTypeID = 5, Rate = 1f }, Product.FactoryTypeIDColumn, Product.ResourceTypeIDColumn, Product.RateColumn);

		yield return new Insert<Ingredient>(new Ingredient() { FactoryTypeID = 2, ResourceTypeID = 1, Rate = 2f }, Ingredient.FactoryTypeIDColumn, Ingredient.ResourceTypeIDColumn, Ingredient.RateColumn);
		yield return new Insert<Ingredient>(new Ingredient() { FactoryTypeID = 3, ResourceTypeID = 2, Rate = 2f }, Ingredient.FactoryTypeIDColumn, Ingredient.ResourceTypeIDColumn, Ingredient.RateColumn);
		yield return new Insert<Ingredient>(new Ingredient() { FactoryTypeID = 4, ResourceTypeID = 2, Rate = 2f }, Ingredient.FactoryTypeIDColumn, Ingredient.ResourceTypeIDColumn, Ingredient.RateColumn);
		yield return new Insert<Ingredient>(new Ingredient() { FactoryTypeID = 5, ResourceTypeID = 2, Rate = 2f }, Ingredient.FactoryTypeIDColumn, Ingredient.ResourceTypeIDColumn, Ingredient.RateColumn);


		yield return new Insert<FactoryTypeMaterial>(new FactoryTypeMaterial() { FactoryTypeID = 1, ResourceTypeID = 1, Count = 10 }, FactoryTypeMaterial.FactoryTypeIDColumn, FactoryTypeMaterial.ResourceTypeIDColumn, FactoryTypeMaterial.CountColumn);
		yield return new Insert<FactoryTypeMaterial>(new FactoryTypeMaterial() { FactoryTypeID = 2, ResourceTypeID = 1, Count = 20 }, FactoryTypeMaterial.FactoryTypeIDColumn, FactoryTypeMaterial.ResourceTypeIDColumn, FactoryTypeMaterial.CountColumn);
		yield return new Insert<FactoryTypeMaterial>(new FactoryTypeMaterial() { FactoryTypeID = 3, ResourceTypeID = 2, Count = 20 }, FactoryTypeMaterial.FactoryTypeIDColumn, FactoryTypeMaterial.ResourceTypeIDColumn, FactoryTypeMaterial.CountColumn);
		yield return new Insert<FactoryTypeMaterial>(new FactoryTypeMaterial() { FactoryTypeID = 4, ResourceTypeID = 2, Count = 20 }, FactoryTypeMaterial.FactoryTypeIDColumn, FactoryTypeMaterial.ResourceTypeIDColumn, FactoryTypeMaterial.CountColumn);
		yield return new Insert<FactoryTypeMaterial>(new FactoryTypeMaterial() { FactoryTypeID = 5, ResourceTypeID = 2, Count = 20 }, FactoryTypeMaterial.FactoryTypeIDColumn, FactoryTypeMaterial.ResourceTypeIDColumn, FactoryTypeMaterial.CountColumn);


		yield return new Insert<Planet>(new Planet() { Name = "test", StockPileBuildDuration = 2 }, Planet.NameColumn, Planet.StockPileBuildDurationColumn);
		yield return new Insert<PlanetResource>(new PlanetResource() { LastCount = 20, LastUpdate = DateTime.Now, PlanetID = 1, ResourceTypeID = 1, Rate = 0 }, PlanetResource.LastCountColumn, PlanetResource.LastUpdateColumn, PlanetResource.PlanetIDColumn, PlanetResource.ResourceTypeIDColumn, PlanetResource.RateColumn);
		yield return new Insert<StockPileMaterial>(new StockPileMaterial() { PlanetID = 1, ResourceTypeID = 1, Count = 10 }, StockPileMaterial.PlanetIDColumn, StockPileMaterial.ResourceTypeIDColumn, StockPileMaterial.CountColumn);

		//yield return new Insert<StockPile>(new StockPile() { PlanetID = 1, AliveDate = DateTime.Now, X = 0, Y = 0 }, StockPile.PlanetIDColumn, StockPile.AliveDateColumn,StockPile.XColumn,StockPile.YColumn);

		//yield return new Insert<Factory>(new Factory() { StockPileID = 1 ,FactoryTypeID = 1, AliveDate = DateTime.Now,X=5, Y = 0 }, Factory.FactoryTypeIDColumn, Factory.AliveDateColumn, Factory.StockPileIDColumn,Factory.XColumn,Factory.YColumn);
	}

*/


}

}
