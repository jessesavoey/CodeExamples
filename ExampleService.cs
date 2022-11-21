namespace Jesse.Services
{
    public class ExampleService : IExampleService
    {
        IDataProvider _data = null;
        ILookUpService _lookUpService = null;
        IUserMapperService _userMapperService = null;
        ILocationService _locationService = null;

        public ExampleService(IDataProvider data, ILookUpService lookUpService, IUserMapperService userMapperService, ILocationService locationService)
        {
            _data = data;
            _lookUpService = lookUpService;
            _userMapperService = userMapperService;
            _locationService = locationService;
        }

        public Example GetById(int id)
        {
            string procName = "[dbo].[Example_SelectById]";
            Example newExample = null;

            _data.ExecuteCmd(procName, delegate (SqlParameterCollection paramCollection)
            {
                paramCollection.AddWithValue("@Id", id);
            }, delegate (IDataReader reader, short set)
            {
                int startingIndex = 0;
                newExample = MapSingleExample(reader, ref startingIndex);
            });
            return newExample;
        }

        public Paged<Example> GetAllPaginated(int pageIndex, int pageSize)
        {
            Paged<Example> pagedResult = null;
            List<Example> results = null;
            int totalCount = 0;
            string procName = "[dbo].[Example_SelectAllPaginated]";


            _data.ExecuteCmd(procName,
                inputParamMapper: delegate (SqlParameterCollection col)
                {
                    col.AddWithValue("@PageIndex", pageIndex);
                    col.AddWithValue("@PageSize", pageSize);
                },
                singleRecordMapper: delegate (IDataReader reader, short set)
                {
                    int index = 0;
                    Example newExample = MapSingleExample(reader, ref index);

                    if (totalCount == 0)
                    {
                        totalCount = reader.GetSafeInt32(index++);
                    }

                    if (results == null)
                    {
                        results = new List<Example>();
                    }

                    results.Add(newExample);

                });

            if (results != null)
            {
                pagedResult = new Paged<Example>(results, pageIndex, pageSize, totalCount);
            }

            return pagedResult;
        }

        public Paged<ExampleV2> GetByOrg(int pageIndex, int pageSize, int orgId)
        {
            Paged<ExampleV2> pagedResult = null;
            List<ExampleV2> results = null;
            int totalCount = 0;
            string procName = "[dbo].[Example_SelectByOrganizationId_V2]";


            _data.ExecuteCmd(procName,
                inputParamMapper: delegate (SqlParameterCollection col)
                {
                    col.AddWithValue("@PageIndex", pageIndex);
                    col.AddWithValue("@PageSize", pageSize);
                    col.AddWithValue("@Id", orgId);
                },
                singleRecordMapper: delegate (IDataReader reader, short set)
                {
                    int index = 0;
                    ExampleV2 newExample = MapSingleExampleV2(reader, ref index);

                    if (totalCount == 0)
                    {
                        totalCount = reader.GetSafeInt32(index++);
                    }

                    if (results == null)
                    {
                        results = new List<ExampleV2>();
                    }

                    results.Add(newExample);

                });

            if (results != null)
            {
                pagedResult = new Paged<ExampleV2>(results, pageIndex, pageSize, totalCount);
            }

            return pagedResult;
        }

        public Paged<Example> GetBySearch(int pageIndex, int pageSize, int orgId, string query)
        {
            Paged<Example> pagedResult = null;
            List<Example> results = null;
            int totalCount = 0;
            string procName = "[dbo].[Example_Search_ByOrganizationId]";


            _data.ExecuteCmd(procName,
                inputParamMapper: delegate (SqlParameterCollection col)
                {
                    col.AddWithValue("@PageIndex", pageIndex);
                    col.AddWithValue("@PageSize", pageSize);
                    col.AddWithValue("@OrgId", orgId);
                    col.AddWithValue("@Query", query);

                },
                singleRecordMapper: delegate (IDataReader reader, short set)
                {
                    int index = 0;
                    Example newExample = MapSingleExample(reader, ref index);

                    if (totalCount == 0)
                    {
                        totalCount = reader.GetSafeInt32(index++);
                    }

                    if (results == null)
                    {
                        results = new List<Example>();
                    }

                    results.Add(newExample);

                });

            if (results != null)
            {
                pagedResult = new Paged<Example>(results, pageIndex, pageSize, totalCount);
            }

            return pagedResult;
        }

        public int Create(ExampleAddRequest model, int userId)
        {
            string procName = "[dbo].[Example_Insert]";
            int id = 0;

            _data.ExecuteNonQuery(procName,
                inputParamMapper: delegate (SqlParameterCollection col)
                {
                    AddCommonParams(col, model, userId);
                    SqlParameter outputId = new SqlParameter("@Id", SqlDbType.Int);
                    outputId.Direction = ParameterDirection.Output;
                    col.Add(outputId);

                }, returnParameters: delegate (SqlParameterCollection returnCol)
                {
                    object idOut = returnCol["@Id"].Value;
                    int.TryParse(idOut.ToString(), out id);
                });

            return id;
        }

        public void Update(ExampleUpdateRequest model, int userId)
        {
            string procName = "[dbo].[Example_Update]";

            _data.ExecuteNonQuery(procName,
                inputParamMapper: delegate (SqlParameterCollection col)
                {
                    col.AddWithValue("@Id", model.Id);
                    AddCommonParams(col, model, userId);
                });
        }

        public void Delete(int id)
        {
            string procName = "[dbo].[Example_DeleteById]";

            _data.ExecuteNonQuery(procName,
                inputParamMapper: delegate (SqlParameterCollection col)
                {
                    col.AddWithValue("@Id", id);
                });
        }

        public int Add(ExampleAddRequest model, int userId)
        {
            int id = 0;

            string procName = "[dbo].[Example_Composite_Insert]";
            DataTable BatchExpertises = null;
            DataTable BatchLicenses = null;
            DataTable BatchCertifications = null;

            BatchExpertises = MapExpertisesToTable(model.BatchExpertises);
            BatchLicenses = MapLicensesToTable(model.BatchExpertises);
            BatchCertifications = MapCertificationsToTable(model.BatchExpertises);

            _data.ExecuteNonQuery(procName, inputParamMapper: delegate (SqlParameterCollection col)
            {
                SqlParameter idOut = new SqlParameter("@Id", SqlDbType.Int);
                idOut.Direction = ParameterDirection.Output;

                AddCommonParams(col, model, userId);
                col.AddWithValue("@BatchExpertises", BatchExpertises);
                col.AddWithValue("@BatchLicenses", BatchLicenses);
                col.AddWithValue("@BatchCertifications", BatchCertifications);
                col.Add(idOut);
                

            }, returnParameters: delegate (SqlParameterCollection returnCollection)
            {
                object oId = returnCollection["@Id"].Value;

                Int32.TryParse(oId.ToString(), out id);
            });
            return id;
        }

        public List<ExampleV2> GetByUserProfileId(int userProfileId)
        {
            string procName = "[dbo].[Example_SelectByUserProfileId]";
            List<ExampleV2> exampleList = null;

            _data.ExecuteCmd(procName, delegate (SqlParameterCollection paramCollection)
            {
                paramCollection.AddWithValue("@UserProfileId", userProfileId);
            }, delegate (IDataReader reader, short set)
            {
                int startingIndex = 0;
                ExampleV2 newExample = MapSingleExampleV2(reader, ref startingIndex);
                if (exampleList == null)
                {
                    exampleList = new List<ExampleV2>();
                }
                exampleList.Add(newExample);
            });
            return exampleList;
        }

        private Example MapSingleExample(IDataReader reader, ref int startIndex)
        {
            Example example = new Example();

            example.Id = reader.GetSafeInt32(startIndex++);
            example.Name = reader.GetSafeString(startIndex++);
            example.Email = reader.GetSafeString(startIndex++);
            example.UserProfile = _userMapperService.MapSingleUserProfile(reader, ref startIndex);
            example.Location = _locationService.MapSingleLocation(reader, ref startIndex);
            example.Phone = reader.GetSafeString(startIndex++);
            example.Industry = _lookUpService.MapSingleLookUp(reader, ref startIndex);
            example.SiteUrl = reader.GetSafeString(startIndex++);
            example.IsActive = reader.GetBoolean(startIndex++);
            example.BatchExpertises = reader.DeserializeObject<List<LookUp>>(startIndex++);
            example.BatchLicenses = reader.DeserializeObject<List<LookUp>>(startIndex++);
            example.BatchCertifications = reader.DeserializeObject<List<LookUp>>(startIndex++);

            return example;
        }

        private ExampleV2 MapSingleExampleV2(IDataReader reader, ref int startIndex)
        {
            ExampleV2 example = new ExampleV2();

            example.Id = reader.GetSafeInt32(startIndex++);
            example.Name = reader.GetSafeString(startIndex++);
            example.Email = reader.GetSafeString(startIndex++);
            example.UserProfile = _userMapperService.MapSingleUserProfile(reader, ref startIndex);
            example.Location = _locationService.MapSingleLocation(reader, ref startIndex);
            example.Phone = reader.GetSafeString(startIndex++);
            example.Industry = _lookUpService.MapSingleLookUp(reader, ref startIndex);
            example.SiteUrl = reader.GetSafeString(startIndex++);
            example.IsActive = reader.GetBoolean(startIndex++);
            example.Expertise = reader.DeserializeObject<List<LookUp>>(startIndex++);
            string organizations = reader.GetSafeString(startIndex++);
            if (!string.IsNullOrEmpty(organizations))
            {
                example.OrganizationsWorkedWith = Newtonsoft.Json.JsonConvert.DeserializeObject<List<OrganizationShort>>(organizations);
            }
            else
            {
                example.OrganizationsWorkedWith = new List<OrganizationShort>();
            }

            return example;
        }

        private static void AddCommonParams(SqlParameterCollection col, ExampleAddRequest model, int userId)
        {
            col.AddWithValue("@FirstName", model.FirstName);
            col.AddWithValue("@LastName", model.LastName);
            col.AddWithValue("@MI", model.MI);
            col.AddWithValue("@AvatarUrl", model.AvatarUrl);
            col.AddWithValue("@Name", model.Name);
            col.AddWithValue("@LocationTypeId", model.LocationTypeId);
            col.AddWithValue("@LineOne", model.LineOne);
            col.AddWithValue("@LineTwo", model.LineTwo);
            col.AddWithValue("@City", model.City);
            col.AddWithValue("@Zip", model.Zip);
            col.AddWithValue("@StateId", model.StateId);
            col.AddWithValue("@Latitude", model.Latitude);
            col.AddWithValue("@Longitude", model.Longitude);
            col.AddWithValue("@Phone", model.Phone);
            col.AddWithValue("@IndustryId", model.IndustryId);
            col.AddWithValue("@SiteUrl", model.SiteUrl);
            col.AddWithValue("@IsActive", model.IsActive);
            col.AddWithValue("@UserId", userId);
        }

        private static DataTable MapExpertisesToTable(List<string> expertiseToMap)
        {

            DataTable table = new DataTable();
            table.Columns.Add("Name", typeof(String));

            if (expertiseToMap != null)
            {
                foreach (string singleExpertise in expertiseToMap)
                {
                    DataRow dr = table.NewRow();
                    int startingIndex = 0;


                    dr.SetField(startingIndex, singleExpertise);

                    table.Rows.Add(dr);
                }
            }
            return table;
        }
        private static DataTable MapLicensesToTable(List<string> licenseToMap)
        {

            DataTable table = new DataTable();
            table.Columns.Add("Name", typeof(String));

            if (licenseToMap != null)
            {
                foreach (string singleLicense in licenseToMap)
                {
                    DataRow dr = table.NewRow();
                    int startingIndex = 0;


                    dr.SetField(startingIndex, singleLicense);

                    table.Rows.Add(dr);
                }
            }
            return table;
        }

        private static DataTable MapCertificationsToTable(List<string> certificationsToMap)
        {

            DataTable table = new DataTable();
            table.Columns.Add("Name", typeof(String));

            if (certificationsToMap != null)
            {
                foreach (string singleCertification in certificationsToMap)
                {
                    DataRow dr = table.NewRow();
                    int startingIndex = 0;


                    dr.SetField(startingIndex, singleCertification);

                    table.Rows.Add(dr);
                }
            }
            return table;
        }
    }
}
