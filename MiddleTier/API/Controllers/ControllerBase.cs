using API.DataverseAccess;
using API.Mappers;
using AutoMapper;
using Microsoft.Extensions.Logging;
using System;

namespace API.Controllers
{
    public class ControllerBase : Microsoft.AspNetCore.Mvc.ControllerBase
    {
        private DVDataAccessFactory dataAccessFactory = null;
        private DVDataAccess adminDataAccess = null;
        private DVDataAccess userDataAccess = null;
        protected IMapper mapper = null;
        protected ILogger logger = null;

        public ControllerBase(MapperConfig mapperconfig, DVDataAccessFactory dataAccessFactory, ILogger<PNBController> logger)
        {
            this.mapper = new Mapper(mapperconfig.mapperConfig);
            this.dataAccessFactory = dataAccessFactory;
            this.logger = logger;
        }

        public DVDataAccess UserDataAccess
        {
            get
            {
                if (userDataAccess == null)
                {
                    userDataAccess = dataAccessFactory.GetUserDataAccess();
                }

                return userDataAccess;
            }
        }

        public DVDataAccess AdminDataAccess
        {
            get
            {
                if (adminDataAccess == null)
                {
                    adminDataAccess = dataAccessFactory.GetAdminDataAccess();
                }

                return adminDataAccess;
            }
        }
    }
}
