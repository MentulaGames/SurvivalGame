[Container: "Creatures"]
{
	[Item: id=0, name="Rabbit"]
	{
		[Texture: 0]
		[Color: R=244, G=164, B=96, A=255]

		[Strength: 2]
		[Dexterity: 16]
		[Intelect: 3]
		[Perception: 10]
		[Endurence: 8]

		[BodyPart: "Head"]
		{
			[TissueLayer: true]
			{
				[Type: "Biomass"]
				[Id: 1]
				[InfluencesEffectiveness: false]
				[Thickness: 2]
				[Area: 30]
			}
		}
	}
	[Item: id=1, name="Human"]
	{
		[Texture: 0]
		[Color: R=244, G=164, B=96, A=255]

		[Strength: 6]
		[Dexterity: 6]
		[Intelect: 6]
		[Perception: 6]
		[Endurence: 6]

		[BodyPart: "Right Leg"]
		{

			[TissueLayer: false]
			{
				[Type: "Biomass"]
				[Id: 0]
				[InfluencesEffectiveness: true]
				[Thickness: 30]
				[Area: 848]
			}

			[TissueLayer: false]
			{
				[Type: "Biomass"]
				[Id: 1]
				[InfluencesEffectiveness: true]
				[Thickness: 60]
				[Area: 3392]
			}

			[TissueLayer: false]
			{
				[Type: "Biomass"]
				[Id: 2]
				[InfluencesEffectiveness: false]
				[Thickness: 1]
				[Area: 5117]
			}
		}

		[BodyPart: "Left Leg"]
		{

			[TissueLayer: false]
			{
				[Type: "Biomass"]
				[Id: 0]
				[InfluencesEffectiveness: true]
				[Thickness: 30]
				[Area: 848]
			}

			[TissueLayer: false]
			{
				[Type: "Biomass"]
				[Id: 1]
				[InfluencesEffectiveness: true]
				[Thickness: 60]
				[Area: 3392]
			}

			[TissueLayer: false]
			{
				[Type: "Biomass"]
				[Id: 2]
				[InfluencesEffectiveness: false]
				[Thickness: 1]
				[Area: 5117]
			}
		}

		[BodyPart: "Right Arm"]
		{

			[TissueLayer: false]
			{
				[Type: "Biomass"]
				[Id: 0]
				[InfluencesEffectiveness: true]
				[Thickness: 20]
				[Area: 565]
			}

			[TissueLayer: false]
			{
				[Type: "Biomass"]
				[Id: 1]
				[InfluencesEffectiveness: true]
				[Thickness: 20]
				[Area: 1507]
			}

			[TissueLayer: false]
			{
				[Type: "Biomass"]
				[Id: 2]
				[InfluencesEffectiveness: false]
				[Thickness: 1]
				[Area: 2036]
			}
		}

		[BodyPart: "Left Arm"]
		{

			[TissueLayer: false]
			{
				[Type: "Biomass"]
				[Id: 0]
				[InfluencesEffectiveness: true]
				[Thickness: 20]
				[Area: 565]
			}

			[TissueLayer: false]
			{
				[Type: "Biomass"]
				[Id: 1]
				[InfluencesEffectiveness: true]
				[Thickness: 20]
				[Area: 1507]
			}

			[TissueLayer: false]
			{
				[Type: "Biomass"]
				[Id: 2]
				[InfluencesEffectiveness: false]
				[Thickness: 1]
				[Area: 2036]
			}
		}

		[BodyPart: "Torso"]
		{

			[TissueLayer: true]
			{
				[Type: "Biomass"]
				[Id: 1]
				[InfluencesEffectiveness: true]
				[Thickness: 86]
				[Area: 1486]
			}

			[TissueLayer: false]
			{
				[Type: "Biomass"]
				[Id: 0]
				[InfluencesEffectiveness: true]
				[Thickness: 20]
				[Area: 3317]
			}

			[TissueLayer: false]
			{
				[Type: "Biomass"]
				[Id: 1]
				[InfluencesEffectiveness: true]
				[Thickness: 20]
				[Area: 4009]
			}
			
			[TissueLayer: false]
			{
				[Type: "Biomass"]
				[Id: 2]
				[InfluencesEffectiveness: false]
				[Thickness: 1]
				[Area: 4372]
			}
		}

		[BodyPart: "Head"]
		{

			[TissueLayer: true]
			{
				[Type: "Biomass"]
				[Id: 1]
				[InfluencesEffectiveness: true]
				[Thickness: 40]
				[Area: 314]
			}

			[TissueLayer: false]
			{
				[Type: "Biomass"]
				[Id: 0]
				[InfluencesEffectiveness: true]
				[Thickness: 10]
				[Area: 707]
			}
			
			[TissueLayer: false]
			{
				[Type: "Biomass"]
				[Id: 2]
				[InfluencesEffectiveness: false]
				[Thickness: 1]
				[Area: 739]
			}
		}
	}
}