﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Jil;

using Ploeh.AutoFixture;

using Swasey.Tests.Schema.Version12;

namespace Swasey.Tests.Helpers
{
    internal static class Fixtures
    {

        public static SingleFixtureBuilder Build
        {
            get { return new SingleFixtureBuilder(CreateAutoFixture()); }
        }

        public static T Create<T>()
        {
            return Build.One.Of<T>();
        }

        public static T Create<T>(T seed)
        {
            return Build.One.Of(seed);
        }

        public static Fixture CreateAutoFixture()
        {
            var fixture = new Fixture();
            fixture.Customize<string>(x => new StringGenerator(() => Guid.NewGuid().ToString("N")));
            return fixture;
        }

        public static string CreateResourceListingJson()
        {
            return JSON.SerializeDynamic(
                new
                {
                    apiVersion = ResourceListingVesion12.ApiVersion,
                    swaggerVersion = SwaggerVersion.Version12.Version(),
                    apis = new[]
                    {
                        new
                        {
                            path = ResourceListingVesion12.Apis_Pet_Path,
                            description = ResourceListingVesion12.Apis_Pet_Description
                        },
                        new
                        {
                            path = ResourceListingVesion12.Apis_User_Path,
                            description = ResourceListingVesion12.Apis_User_Description
                        },
                        new
                        {
                            path = ResourceListingVesion12.Apis_Store_Path,
                            description = ResourceListingVesion12.Apis_Store_Description
                        }
                    },
                    authorizations = new Dictionary<string, dynamic>
                    {
                        {
                            ResourceListingVesion12.Authorization_Type_OAuth2, new
                            {
                                type = ResourceListingVesion12.Authorization_Type_OAuth2,
                                scopes = new[]
                                {
                                    new
                                    {
                                        scope = ResourceListingVesion12.Scope_Email,
                                        description = ResourceListingVesion12.Scope_Email_Description
                                    },
                                    new
                                    {
                                        scope = ResourceListingVesion12.Scope_Pets,
                                        description = ResourceListingVesion12.Scope_Pets_Description
                                    }
                                },
                                grantTypes = new
                                {
                                    @implicit = new
                                    {
                                        loginEndpoint = new
                                        {
                                            url = ResourceListingVesion12.Url_LoginEndpoint
                                        }
                                    },
                                    authorization_code = new
                                    {
                                        tokenRequestEndpoint = new
                                        {
                                            url = ResourceListingVesion12.Url_TokenRequestEndpoint,
                                            clientIdName = ResourceListingVesion12.TokenRequestEndpoint_ClientIdName,
                                            clientSecretName = ResourceListingVesion12.TokenRequestEndpoint_ClientSecretName
                                        },
                                        tokenEndpoint = new
                                        {
                                            url = ResourceListingVesion12.Url_TokenEndpoint,
                                            tokenName = ResourceListingVesion12.TokenEndpoint_TokenName
                                        }
                                    }
                                }
                            }
                        }
                    },
                    info = new
                    {
                        title = ResourceListingVesion12.Info_Title,
                        description = ResourceListingVesion12.Info_Description,
                        termsOfServiceUrl = ResourceListingVesion12.Url_TermsOfServiceUrl,
                        contact = ResourceListingVesion12.Info_Contact,
                        license = ResourceListingVesion12.Info_License,
                        licenseUrl = ResourceListingVesion12.Url_LicenseUrl
                    }
                });
        }

        public static Task<string> GenerateApiJson(string path)
        {
            dynamic apiObject = null;
            if (ResourceListingVesion12.Apis_Store_Path.Equals(path, StringComparison.InvariantCultureIgnoreCase))
            {
                apiObject = GenerateStoreApiObject(path);
            }
            if (ResourceListingVesion12.Apis_Pet_Path.Equals(path, StringComparison.InvariantCultureIgnoreCase))
            {
                apiObject = GeneratePetApiObject(path);
            }
            if (ResourceListingVesion12.Apis_User_Path.Equals(path, StringComparison.InvariantCultureIgnoreCase))
            {
                apiObject = GenerateUserApiObject(path);
            }

            if (apiObject == null)
            {
                throw new Exception("I don't recognize that path: " + path);
            }

            return Task.FromResult(JSON.SerializeDynamic(apiObject));
        }

        public static dynamic GeneratePetApiObject(string resourcePath)
        {
            return new
            {
                apiVersion = ResourceListingVesion12.ApiVersion,
                swaggerVersion = SwaggerVersion.Version12.Version(),
                basePath = "/",
                resourcePath = resourcePath,
                produces = new[] {"application/json"},
                authorizations = new
                {},
                apis = new object[]
                {
                    new
                    {
                        path = resourcePath + "/{petId}",
                        operations = new object[]
                        {
                            new
                            {
                                method = "GET",
                                summary = "Find pet by ID",
                                notes = "Returns a pet based on ID",
                                type = "Pet",
                                nickname = "getPetById",
                                authorizations = new {},
                                parameters = new object[]
                                {
                                    new
                                    {
                                        name =  "petId",
                                        description = "ID of pet that needs to be fetched",
                                        required = true,
                                        type = "integer",
                                        format = "int64",
                                        paramType = "path",
                                        allowMultiple = false,
                                        minimum = "1.0",
                                        maximum = "100000.0"
                                    }
                                },
                                responseMessages = new object[]
                                {
                                    new
                                    {
                                        code = 400,
                                        message = "Invalid ID supplied"
                                    },
                                    new
                                    {
                                        code = 404,
                                        message = "Pet not found"
                                    }
                                }
                            },
                            new
                            {
                                method = "PUT",
                                summary = "Update an existing pet",
                                notes = "",
                                type = "void",
                                nickname = "updatePet",
                                authorizations = new {},
                                parameters = new object[]
                                {
                                    new
                                    {
                                        name = "body",
                                        description = "Pet object that needs to be updated in the store",
                                        required = true,
                                        type = "Pet",
                                        paramType = "body"
                                    }
                                },
                                responseMessages = new object[]
                                {
                                    new
                                    {
                                        code = 400,
                                        message = "Invalid ID supplied"
                                    },
                                    new
                                    {
                                        code = 404,
                                        message = "Pet not found"
                                    },
                                    new
                                    {
                                        code = 405,
                                        message = "Validation exception"
                                    }
                                }
                            },
                            new
                            {
                                method = "POST",
                                summary = "Updtes a pet in the store with form data",
                                notes = "",
                                type = "void",
                                nickname = "updatePetWithForm",
                                consumes = new []
                                {
                                    "application/x-www-form-urlencoded"
                                },
                                authorizations = new
                                {
                                    oauth2 = new[]
                                    {
                                        new
                                        {
                                            scope = "write:pets",
                                            description = "modify pets in your account"
                                        }
                                    }
                                },
                                parameters = new object[]
                                {
                                    new
                                    {
                                        name = "petId",
                                        description = "ID of the pet that needs to be updated",
                                        required = true,
                                        type = "string",
                                        paramType = "path",
                                        allowMultiple = false
                                    },
                                    new
                                    {
                                        name = "name",
                                        description = "Updated name of the pet",
                                        required = false,
                                        type = "string",
                                        paramType = "form",
                                        allowMultiple = false
                                    },
                                    new
                                    {
                                        name = "status",
                                        description = "Updated status of the pet",
                                        required = false,
                                        type = "string",
                                        paramType = "form",
                                        allowMultiple = false
                                    }
                                },
                                responseMessages = new[]
                                {
                                    new
                                    {
                                        code = 405,
                                        message = "Invalid input"
                                    }
                                }
                            }
                        }
                    }
                },
                models = new
                {
                    Pet = new
                    {
                        id = "Pet",
                        required = new[]
                        {
                            "id", "name"
                        },
                        properties = new
                        {
                            id = new
                            {
                                type = "integer",
                                format = "int64",
                                description = "unique identifier for the pet",
                                minimum = "0.0",
                                maximum = "100.0"
                            },
                            category = new Dictionary<string, dynamic>
                            {
                                {"$ref", "Category" }
                            },
                            name = new
                            {
                                type = "string"
                            },
                            photoUrls = new
                            {
                                type = "array",
                                items = new
                                {
                                    type = "string"
                                }
                            },
                            tags = new
                            {
                                type = "array",
                                items = new Dictionary<string, string>
                                {
                                    {"ref", "Tag"}
                                }
                            },
                            status = new
                            {
                                type = "string",
                                description = "pet status in the store",
                                @enum = new[]
                                {
                                    "available",
                                    "pending",
                                    "sold"
                                }
                            }
                        }
                    },
                    Tag = new
                    {
                        id = "Tag",
                        properties = new
                        {
                            id = new
                            {
                                type = "integer",
                                format = "int64"
                            },
                            name = new
                            {
                                type = "string"
                            }
                        }
                    },
                    Category = new
                    {
                        id = "Category",
                        properties = new
                        {
                            id = new
                            {
                                type = "integer",
                                format = "int64"
                            },
                            name = new
                            {
                                type = "string"
                            }
                        }
                    }
                }
            };
        }

        public static dynamic GenerateStoreApiObject(string resourcePath)
        {
            return new
            {
                apiVersion = ResourceListingVesion12.ApiVersion,
                swaggerVersion = SwaggerVersion.Version12.Version(),
                basePath = "/",
                resourcePath = resourcePath,
                produces = new[] {"application/json"},
                authorizations = new
                {},
                apis = new object[]
                {
                    new
                    {
                        path = resourcePath + "/order/{orderId}",
                        operations = new object[]
                        {
                            new
                            {
                                method = "GET",
                                summary = "Find purchase order by ID",
                                notes = "For valid response try integer IDs with value <= 5. Anything above 5 or nonintegers will generate API errors",
                                type = "Order",
                                nickname = "getOrderById",
                                authorizations = new
                                {},
                                parameters = new[]
                                {
                                    new
                                    {
                                        name = "orderId",
                                        description = "ID of pet that needs to be fetched",
                                        required = true,
                                        type = "string",
                                        paramType = "path"
                                    }
                                },
                                responseMessages = new[]
                                {
                                    new
                                    {
                                        code = 400,
                                        message = "Invalid ID supplied"
                                    },
                                    new
                                    {
                                        code = 404,
                                        message = "Order not found"
                                    }
                                }
                            },
                            new
                            {
                                method = "DELETE",
                                summary = "Delete purchase order by ID",
                                notes = "For valid response try integer IDs with value < 1000. Anything above 1000 or nonintegers will generate API errors",
                                type = "void",
                                nickname = "deleteOrder",
                                authorizations = new
                                {
                                    oauth2 = new[]
                                    {
                                        new
                                        {
                                            scope = "test:anything",
                                            description = "anything"
                                        }
                                    }
                                },
                                parameters = new[]
                                {
                                    new
                                    {
                                        name = "orderId",
                                        description = "ID of the order that needs to be deleted",
                                        required = true,
                                        type = "string",
                                        paramType = "path"
                                    }
                                },
                                responseMessages = new[]
                                {
                                    new
                                    {
                                        code = 400,
                                        message = "Invalid ID supplied"
                                    },
                                    new
                                    {
                                        code = 404,
                                        message = "Order not found"
                                    }
                                }
                            }
                        }
                    },
                    new
                    {
                        path = resourcePath + "/order",
                        operations = new object[]
                        {
                            new
                            {
                                method = "POST",
                                summary = "Place an order for a pet",
                                notes = "",
                                type = "void",
                                nickname = "placeOrder",
                                authorizations = new
                                {
                                    oauth2 = new object[]
                                    {
                                        new
                                        {
                                            scope = "test:anything",
                                            description = "anything"
                                        }
                                    }
                                },
                                parameters = new object[]
                                {
                                    new
                                    {
                                        name = "body",
                                        description = "order placed for purchasing the pet",
                                        required = true,
                                        type = "Order",
                                        paramType = "body"
                                    }
                                },
                                responseMessages = new object[]
                                {
                                    new
                                    {
                                        code = 400,
                                        message = "Invalid order"
                                    }
                                }
                            }
                        }
                    }
                },
                models = new
                {
                    Order = new
                    {
                        id = "Order",
                        properties = new
                        {
                            id = new
                            {
                                type = "integer",
                                format = "int64"
                            },
                            petId = new
                            {
                                type = "integer",
                                format = "in32"
                            },
                            status = new
                            {
                                type = "string",
                                description = "Order Status",
                                @enum = new[]
                                {
                                    "placed",
                                    "approved",
                                    "delivered"
                                }
                            },
                            shipDate = new
                            {
                                type = "string",
                                format = "date-time"
                            }
                        }
                    }
                }
            };
        }

        public static dynamic GenerateUserApiObject(string resourcePath)
        {
            return new
            {
                apiVersion = ResourceListingVesion12.ApiVersion,
                swaggerVersion = SwaggerVersion.Version12.Version(),
                basePath = "/",
                resourcePath = resourcePath,
                produces = new[] {"application/json"},
                authorizations = new
                {},
                apis = new object[]
                {
                    new
                    {
                        path = resourcePath,
                        operations = new object[]
                        {
                            new
                            {
                                method = "POST",
                                summary = "Create user",
                                notes = "This can only be done by the logged in user.",
                                type = "void",
                                nickname = "createUser",
                                authorizations = new
                                {
                                    oauth2 = new object[]
                                    {
                                        new
                                        {
                                            scope = "test:anything",
                                            description = "anything"
                                        }
                                    }
                                },
                                parameters = new object[]
                                {
                                    new
                                    {
                                        name = "body",
                                        description = "Created user object",
                                        required = true,
                                        type = "User",
                                        paramType = "body",
                                        allowMultiple = false
                                    }
                                }
                            }
                        }
                    },
                    new
                    {
                        path = resourcePath + "/createWithArray",
                        operations = new object[]
                        {
                            new
                            {
                                method = "POST",
                                summary = "Creates lists of users with given input array",
                                notes = "",
                                type = "void",
                                nickname = "createUsersWithArrayInput",
                                authorizations = new
                                {
                                    oauth2 = new object[]
                                    {
                                        new
                                        {
                                            scope = "test:anything",
                                            description = "anything"
                                        }
                                    }
                                },
                                parameters = new object[]
                                {
                                    new
                                    {
                                        name = "body",
                                        description = "List of user objects",
                                        required = true,
                                        type = "array",
                                        items = new Dictionary<string, string>
                                        {
                                            {"$ref", "User"}
                                        },
                                        paramType = "body",
                                        allowMultiple = false
                                    }
                                }
                            }
                        }
                    },
                    new
                    {
                        path = resourcePath + "/createWithList",
                        operations = new object[]
                        {
                            new
                            {
                                method = "POST",
                                summary = "Creates lists of users with given input list",
                                notes = "",
                                type = "void",
                                nickname = "createUsersWithListInput",
                                authorizations = new
                                {
                                    oauth2 = new object[]
                                    {
                                        new
                                        {
                                            scope = "test:anything",
                                            description = "anything"
                                        }
                                    }
                                },
                                parameters = new object[]
                                {
                                    new
                                    {
                                        name = "body",
                                        description = "List of user objects",
                                        required = true,
                                        type = "array",
                                        items = new Dictionary<string, string>
                                        {
                                            {"$ref", "User"}
                                        },
                                        paramType = "body",
                                        allowMultiple = false
                                    }
                                }
                            }
                        }
                    },
                    new
                    {
                        path = resourcePath + "/{username}",
                        operations = new object[]
                        {
                            new
                            {
                                method = "PUT",
                                summary = "Update user",
                                notes = "This can only be done by the logged in user.",
                                type = "void",
                                nickname = "updateUser",
                                authorizations = new
                                {
                                    oauth2 = new object[]
                                    {
                                        new
                                        {
                                            scope = "test:anything",
                                            description = "anything"
                                        }
                                    }
                                },
                                parameters = new object[]
                                {
                                    new
                                    {
                                        name = "username",
                                        description = "name of the user to update",
                                        required = true,
                                        type = "string",
                                        paramType = "path",
                                        allowMultiple = false
                                    },
                                    new
                                    {
                                        name = "body",
                                        description = "Updated user object",
                                        required = true,
                                        type = "User",
                                        paramType = "body",
                                        allowMultiple = false
                                    }
                                },
                                responseMessages = new[]
                                {
                                    new
                                    {
                                        code = 400,
                                        message = "Invalid username supplied"
                                    },
                                    new
                                    {
                                        code = 404,
                                        message = "User not found"
                                    }
                                }
                            },
                            new
                            {
                                method = "DELETE",
                                summary = "Delete user",
                                notes = "This can only be done by the logged in user.",
                                type = "void",
                                nickname = "deleteUser",
                                authorizations = new
                                {
                                    oauth2 = new object[]
                                    {
                                        new
                                        {
                                            scope = "test:anything",
                                            description = "anything"
                                        }
                                    }
                                },
                                parameters = new object[]
                                {
                                    new
                                    {
                                        name = "username",
                                        description = "name of the user to update",
                                        required = true,
                                        type = "string",
                                        paramType = "path",
                                        allowMultiple = false
                                    }
                                },
                                responseMessages = new[]
                                {
                                    new
                                    {
                                        code = 400,
                                        message = "Invalid username supplied"
                                    },
                                    new
                                    {
                                        code = 404,
                                        message = "User not found"
                                    }
                                }
                            }
                        }
                    }
                },
                models = new
                {
                    User = new
                    {
                        id = "User",
                        properties = new
                        {
                            id = new
                            {
                                type = "integer",
                                format = "int64"
                            },
                            firstName = new
                            {
                                type = "string"
                            },
                            username = new
                            {
                                type = "string"
                            },
                            lastName = new
                            {
                                type = "string"
                            },
                            email = new
                            {
                                type = "string"
                            },
                            password = new
                            {
                                type = "string"
                            },
                            phone = new
                            {
                                type = "string"
                            },
                            userStatus = new
                            {
                                type = "integer",
                                format = "int32",
                                description = "User Status",
                                @enum = new[]
                                {
                                    "1-registered",
                                    "2-active",
                                    "3-closed"
                                }
                            }
                        }
                    }
                }
            };
        }

    }
}