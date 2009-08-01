using NBehave.Spec.NUnit;
using NUnit.Framework;

namespace AutoMapper.UnitTests
{
	namespace NullBehavior
	{
		public class When_mapping_a_model_with_null_items : AutoMapperSpecBase
		{
			private ModelDto _result;

			private class ModelDto
			{
				public ModelSubDto Sub { get; set; }
				public int SubSomething { get; set; }
				public string NullString { get; set; }
			}

			private class ModelSubDto
			{
				public int[] Items { get; set; }
			}

			private class ModelObject
			{
				public ModelSubObject Sub { get; set; }
				public string NullString { get; set; }
			}

			private class ModelSubObject
			{
				public int[] GetItems()
				{
					return new[] { 0, 1, 2, 3 };
				}

				public int Something { get; set; }
			}

			protected override void Establish_context()
			{
				var model = new ModelObject();
				model.Sub = null;

				Mapper.AllowNullDestinationValues = false;
				Mapper.CreateMap<ModelObject, ModelDto>();
				Mapper.CreateMap<ModelSubObject, ModelSubDto>();

				_result = Mapper.Map<ModelObject, ModelDto>(model);
			}

			[Test]
			public void Should_populate_dto_items_with_a_value()
			{
				_result.Sub.ShouldNotBeNull();
			}

			[Test]
			public void Should_provide_empty_array_for_array_type_values()
			{
				_result.Sub.Items.ShouldNotBeNull();
			}

			[Test]
			public void Should_return_default_value_of_property_in_the_chain()
			{
				_result.SubSomething.ShouldEqual(0);
			}

			[Test]
			public void Default_value_for_string_should_be_empty()
			{
				_result.NullString.ShouldEqual(string.Empty);
			}
		}

		public class When_overriding_null_behavior_with_null_source_items : AutoMapperSpecBase
		{
			private ModelDto _result;

			private class ModelDto
			{
				public ModelSubDto Sub { get; set; }
				public int SubSomething { get; set; }
				public string NullString { get; set; }
			}

			private class ModelSubDto
			{
				public int[] Items { get; set; }
			}

			private class ModelObject
			{
				public ModelSubObject Sub { get; set; }
				public string NullString { get; set; }
			}

			private class ModelSubObject
			{
				public int[] GetItems()
				{
					return new[] { 0, 1, 2, 3 };
				}

				public int Something { get; set; }
			}

			protected override void Establish_context()
			{
				var model = new ModelObject();
				model.Sub = null;
				model.NullString = null;

				Mapper.Initialize(c => c.AllowNullDestinationValues = true);
				Mapper.CreateMap<ModelObject, ModelDto>();
				Mapper.CreateMap<ModelSubObject, ModelSubDto>();

				_result = Mapper.Map<ModelObject, ModelDto>(model);
			}

			[Test]
			public void Should_map_first_level_items_as_null()
			{
				_result.NullString.ShouldBeNull();
			}

			[Test]
			public void Should_map_primitive_items_as_default()
			{
				_result.SubSomething.ShouldEqual(0);
			}

			[Test]
			public void Should_map_any_sub_mapped_items_as_null()
			{
				_result.Sub.ShouldBeNull();
			}
		}

		public class When_overriding_null_behavior_in_a_profile : AutoMapperSpecBase
		{
			private DefaultDestination _defaultResult;
			private NullDestination _nullResult;

			private class DefaultSource
			{
				public object Value { get; set; }
			}

			private class DefaultDestination
			{
				public object Value { get; set; }
			}
			
			private class NullSource
			{
				public object Value { get; set; }
			}

			private class NullDestination
			{
				public object Value { get; set; }
			}

			protected override void Establish_context()
			{
				Mapper.CreateProfile("MapsNulls", p =>
					{
						p.AllowNullDestinationValues = false;
						p.CreateMap<NullSource, NullDestination>();
					});
				Mapper.CreateMap<DefaultSource, DefaultDestination>();
			}

			protected override void Because_of()
			{
				_defaultResult = Mapper.Map<DefaultSource, DefaultDestination>(new DefaultSource());
				_nullResult = Mapper.Map<NullSource, NullDestination>(new NullSource());
			}

			[Test]
			public void Should_use_default_behavior_in_default_profile()
			{
				_defaultResult.Value.ShouldBeNull();
			}

			[Test]
			public void Should_use_overridden_null_behavior_in_profile()
			{
				_nullResult.Value.ShouldNotBeNull();
			}
		}

	}
}