namespace RestBucks.Data
{
  using System.Collections.Generic;
  using System.Linq;
  using System.Reflection;
  using Infrastructure.Domain;
  using Infrastructure.Domain.BaseClass;
  using NHibernate.Cfg.MappingSchema;
  using NHibernate.Mapping.ByCode;
  using Orders.Domain;
  using Products.Domain;

  public class Mapper
  {
    public static HbmMapping Generate()
    {
      //Conventions
      var mapper = new ConventionModelMapper();
      var baseEntity = typeof (EntityBase);

      mapper.BeforeMapProperty += (ispector, member, customizer) => customizer.Length(40);

      mapper.BeforeMapManyToOne +=
        (insp, prop, map) => map.Column(prop.LocalMember.GetPropertyOrFieldType().Name + "Id");

      mapper.BeforeMapManyToOne +=
        (insp, prop, map) => map.Cascade(Cascade.Persist);

      mapper.BeforeMapBag +=
        (insp, prop, map) => map.Key(km => km.Column(prop.GetContainerEntity(insp).Name + "Id"));

      mapper.BeforeMapSet +=
        (insp, prop, map) => map.Key(km => km.Column(prop.GetContainerEntity(insp).Name + "Id"));

      mapper.IsEntity((t, d) => baseEntity.IsAssignableFrom(t) && baseEntity != t);

      mapper.IsRootEntity((t, d) => t.BaseType == baseEntity);

      mapper.IsSet(IsSetFieldType);

      Customize(mapper);

      HbmMapping mappings = mapper.CompileMappingFor(new[]
                                                     {
                                                       typeof (Customization), typeof (Order),
                                                       typeof (Payment),
                                                       typeof (OrderItem), typeof (Product)
                                                     });
      return mappings;
    }


    private static bool IsSetFieldType(MemberInfo mi, bool declared)
    {
      var propertyTypeIsSet = mi.GetPropertyOrFieldType()
        .GetGenericIntercafesTypeDefinitions()
        .Contains(typeof (ISet<>));

      if (propertyTypeIsSet) return true;

      var backFieldInfo = PropertyToField.GetBackFieldInfo((PropertyInfo) mi);

      return backFieldInfo != null
             && backFieldInfo
                  .FieldType.GetGenericIntercafesTypeDefinitions().Contains(typeof (ISet<>));
    }

    private static void Customize(ModelMapper mapper)
    {
      mapper.Class<EntityBase>(map =>
                               {
                                 map.Id(e => e.Id, id => id.Generator(Generators.HighLow));
                                 map.Version(e => e.Version, d => { });
                               });
      mapper.Class<Product>(map => map.Set(p => p.Customizations,
                                           set => set.Cascade(Cascade.All),
                                           rel => rel.ManyToMany()));

      mapper.Class<Customization>(map => map.Set(p => p.PossibleValues,
                                                 set => set.Cascade(Cascade.All),
                                                 rel => rel.Element()));

      mapper.Class<Order>(map =>
                          {
                            map.Set(p => p.Items, set =>
                                                  {
                                                    set.Cascade(Cascade.All);
                                                    set.Inverse(true);
                                                  });
                            map.ManyToOne(o => o.Payment, o => o.Cascade(Cascade.All));
                          });
    }
  }
}