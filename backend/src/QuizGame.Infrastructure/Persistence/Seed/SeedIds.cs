namespace QuizGame.Infrastructure.Persistence.Seed;

public static class SeedIds
{
    public static readonly Guid EasyCategoryId = Guid.Parse("59ef08e6-1514-4379-87a1-c1b802d616ba");
    public static readonly Guid IntermediateCategoryId = Guid.Parse("36cafaf0-b7cd-4556-9ac7-4b5b83f39118");
    public static readonly Guid HardCategoryId = Guid.Parse("1ab48485-17e9-4eae-bcf6-113bbf0135c6");

    public static readonly IReadOnlyList<Guid> EasyQuestionIds =
    [
        Guid.Parse("bca1ad0d-c8e0-48f7-a83e-39d71540c206"),
        Guid.Parse("2a32cdc7-1c3a-4d84-b9e0-06ce7fca26ac"),
        Guid.Parse("095aa9ae-8d60-46a1-bba6-78e9fa6c86e7"),
        Guid.Parse("7992d28c-4399-4180-a957-8755f76e1184"),
        Guid.Parse("427c79c8-d4b8-48c4-9308-b7eb1ddf140e"),
        Guid.Parse("a4999ba8-15f3-4585-844a-0f0e1f17dcee")
    ];

    public static readonly IReadOnlyList<Guid> IntermediateQuestionIds =
    [
        Guid.Parse("3fa633a4-6979-45a9-a589-cf8c2754b6ec"),
        Guid.Parse("3c933504-e478-4984-9044-365bd27ccb6c"),
        Guid.Parse("21e727f6-2da0-401a-9211-6ca24a85648f"),
        Guid.Parse("93a07525-cd78-42dd-bead-420e1cf7db0e"),
        Guid.Parse("990bd4b0-5e85-497e-a19e-2d4ef3d1263e"),
        Guid.Parse("41905891-a86f-44f1-b8f4-3bdc12105fc0")
    ];

    public static readonly IReadOnlyList<Guid> HardQuestionIds =
    [
        Guid.Parse("7d196c04-6a14-4946-959a-17166e0ad1f2"),
        Guid.Parse("5d34c73c-1f5a-4685-ad54-bee58fbf677d"),
        Guid.Parse("b85875b3-e141-4392-9447-58d2f2efbdd9"),
        Guid.Parse("a2e5ba2d-6dc1-47df-aeeb-e1540aa644ed"),
        Guid.Parse("4917bb8d-f18b-4afa-b437-4da489b75567"),
        Guid.Parse("45080bdc-002e-40b3-9027-15e764ff5c63")
    ];

    public static readonly IReadOnlyList<IReadOnlyList<Guid>> EasyAnswerIds =
    [
        [
            Guid.Parse("6a740a2b-f51a-4927-bc7a-e4e65d875225"),
            Guid.Parse("8faea26f-c49a-4353-b580-36eaa695f69b"),
            Guid.Parse("d4db326d-da94-4123-bfde-1b73c44d51a5"),
            Guid.Parse("e41bfa0b-4c9d-4698-bb4e-4f69a4ef3d3b")
        ],
        [
            Guid.Parse("b76cd511-1cbc-4ccb-9745-83f1dca17d88"),
            Guid.Parse("6ede8f53-90d2-4bf2-a12b-fa23aeb867b0"),
            Guid.Parse("6df053aa-fd38-4a71-afd0-28a320ac566a"),
            Guid.Parse("c8b6dc4e-4ec2-4c07-9e40-097c01176227")
        ],
        [
            Guid.Parse("32a71eb0-394b-4411-b995-bb8204edf856"),
            Guid.Parse("0c62b880-f2b6-49c3-ae54-549aed98786e"),
            Guid.Parse("14b94f04-b857-46e3-bc6f-8f37b237f363"),
            Guid.Parse("8fc0d028-64ba-48d8-8c57-b4a3b209ef91")
        ],
        [
            Guid.Parse("6c409052-d21b-4c9b-90a5-e85c76fda3eb"),
            Guid.Parse("7bec1930-def7-47ac-84b1-11de96b87c87"),
            Guid.Parse("b3ab0076-11ea-4df7-b14f-ebb685adad09"),
            Guid.Parse("dc198a2a-0e14-4605-8540-f0cac3724a13")
        ],
        [
            Guid.Parse("0709943d-63f7-4d98-b4e7-89a62c8c690c"),
            Guid.Parse("d949a619-63f4-4487-879d-2bccc39118b3"),
            Guid.Parse("c28194f2-bf5e-408a-83f3-ae4abf295e2b"),
            Guid.Parse("b209bea0-cc42-44cb-9643-ab885a46c6fa")
        ],
        [
            Guid.Parse("46f12667-a4bd-4183-bd36-d24c7c84e068"),
            Guid.Parse("58155e79-65f0-4ecb-9f0c-bc278f2169a3"),
            Guid.Parse("5b97921c-55f9-49f4-80cf-32240dffb935"),
            Guid.Parse("5d13aff7-6d70-4707-a825-408ccc728844")
        ]
    ];

    public static readonly IReadOnlyList<IReadOnlyList<Guid>> IntermediateAnswerIds =
    [
        [
            Guid.Parse("cfcec57d-dfb5-40a9-8be9-be3b0066fa05"),
            Guid.Parse("357fd4f3-28a7-42d9-8ed3-9eeea8b3651f"),
            Guid.Parse("26f60334-ec80-444b-9099-514204cdac89"),
            Guid.Parse("1cb66463-8ecd-403f-af9b-9df36c213021")
        ],
        [
            Guid.Parse("8500a296-cb9e-4e97-8c5c-b2f2d96ec446"),
            Guid.Parse("e3f1ab62-6c00-4aa1-ba53-7ad08e2c09c8"),
            Guid.Parse("a4aee812-2a6e-4a17-83f5-4942c3db8f1b"),
            Guid.Parse("e01251eb-a282-4550-9fd6-4e4731840988")
        ],
        [
            Guid.Parse("fadc3e62-c0f6-4222-a183-829af84327b4"),
            Guid.Parse("e4b1ec49-581d-47a3-8539-793fd038e771"),
            Guid.Parse("0c7c8a6e-68ec-4859-b8ab-122c3d2e0f3c"),
            Guid.Parse("d8503a01-3339-46ae-95c6-5f713b0e84f0")
        ],
        [
            Guid.Parse("0551a798-9577-4fc9-81fd-0fdf6907720f"),
            Guid.Parse("d8e13761-ad7c-4857-9728-cfee81cc129a"),
            Guid.Parse("197fafb0-891c-404b-a5f8-4ce22e22f7d3"),
            Guid.Parse("3560622c-6851-43dd-962c-ce8c59d41d85")
        ],
        [
            Guid.Parse("65e31c46-cb67-4d38-9dc2-695322fc953a"),
            Guid.Parse("fe6fd12a-ce84-4c4a-82ce-a66c9f5ab717"),
            Guid.Parse("9825d3fd-0cf7-4148-bf91-4b5fe92a1d52"),
            Guid.Parse("583d0f0b-40f5-4a1f-b199-f8b78d60a099")
        ],
        [
            Guid.Parse("3fa01924-ac0c-4276-90ab-a6c3407aa943"),
            Guid.Parse("4aef8f31-4732-4fd0-9205-47ae7c4c6d7e"),
            Guid.Parse("b82290b2-ece9-4510-ab39-9043ecaa4e05"),
            Guid.Parse("2246117f-ffef-47d2-87e7-6c6e95485867")
        ]
    ];

    public static readonly IReadOnlyList<IReadOnlyList<Guid>> HardAnswerIds =
    [
        [
            Guid.Parse("02e173bd-e815-412b-8e4a-c1a29ed15933"),
            Guid.Parse("d3334534-dc85-4d1e-a526-f6f8fb915895"),
            Guid.Parse("493e677c-02e7-44ee-b46f-41326ec5bcce"),
            Guid.Parse("3b60fd8b-7b44-46a4-89a4-6771d4ec1995")
        ],
        [
            Guid.Parse("9b91d187-6b96-483b-a3b7-ce4a51bdf5fe"),
            Guid.Parse("206b1a69-23b1-424b-95a7-c744fa847946"),
            Guid.Parse("39ec0665-b326-4be1-b7fd-6d55fef959c1"),
            Guid.Parse("4c8d96ab-3e97-4d49-bc59-a404d215ada1")
        ],
        [
            Guid.Parse("d8a06579-66d7-434e-b37d-38037079f444"),
            Guid.Parse("766fd5ca-8bf8-4ab0-9152-9979f44cc6af"),
            Guid.Parse("7f43030c-1af6-453e-bb37-0944198860a7"),
            Guid.Parse("c3e6a42e-2860-47f9-8187-abf6f6d548e2")
        ],
        [
            Guid.Parse("d10505ae-96ab-4099-9a30-3a96021658a5"),
            Guid.Parse("7165f901-b72f-432a-a266-9cf5f9c44e62"),
            Guid.Parse("c62e8a06-f2c2-4ba6-bc4c-76367d2503a6"),
            Guid.Parse("0f37d8d4-e79f-46d6-9dd1-3a1bb07c199a")
        ],
        [
            Guid.Parse("ef215a79-7aad-4013-864e-bf764a007a9c"),
            Guid.Parse("d0f86c95-7cb3-4e86-a922-3f8fc67cc445"),
            Guid.Parse("e1532ac7-042e-4653-84d3-be7f8ea5302e"),
            Guid.Parse("3145ff07-0517-49a6-bb02-96e59b490d49")
        ],
        [
            Guid.Parse("224fab6e-24cb-4902-a89a-bc02cf2dfe3d"),
            Guid.Parse("7f768266-e394-4fbe-9c29-55e5c6ef418b"),
            Guid.Parse("f671855e-9d4b-4571-b6e8-e38ad8101aa1"),
            Guid.Parse("d125b2ae-cf90-4051-9f27-c65084125f25")
        ]
    ];
}
