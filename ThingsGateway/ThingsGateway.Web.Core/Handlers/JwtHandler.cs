namespace ThingsGateway.Api.Core
{
    public class JwtHandler : AppAuthorizeHandler
    {
        /// <summary>
        /// 自动刷新Token
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task HandleAsync(AuthorizationHandlerContext context)
        {
            if (JWTEncryption.AutoRefreshToken(context, context.GetCurrentHttpContext(), App.GetOptions<JWTSettingsOptions>().ExpiredTime, App.GetOptions<RefreshTokenOptions>().ExpiredTime))
            {
                await AuthorizeHandleAsync(context);
            }
            else
            {
                context.Fail(); // 授权失败
                DefaultHttpContext currentHttpContext = context.GetCurrentHttpContext();
                if (currentHttpContext == null)
                    return;
                currentHttpContext.SignoutToSwagger();
            }
        }

        public override async Task<bool> PipelineAsync(AuthorizationHandlerContext context, DefaultHttpContext httpContext)
        {
            // 已自动验证 Jwt Token 有效性
            return await CheckAuthorzieAsync(httpContext);
        }

        /// <summary>
        /// 检查权限
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        private static async Task<bool> CheckAuthorzieAsync(DefaultHttpContext httpContext)
        {
            // 第三方授权模式
            if (App.User.FindFirst(ClaimConst.RunMode)?.Value == ((int)RunModeEnum.OpenID).ToString())
                return true;

            // 排除超管
            if (App.User.FindFirst(ClaimConst.AccountType)?.Value == ((int)AccountTypeEnum.SuperAdmin).ToString())
                return true;

            // 路由名称
            var routeName = httpContext.Request.Path.Value[1..].Replace("/", ":");
            if (httpContext.Request.Path.StartsWithSegments("/api"))
                routeName = httpContext.Request.Path.Value[5..].Replace("/", ":");

            // 默认路由
            var defaultRoutes = new List<string>()
            {
                "userInfo",  // 获取用户信息
                "loginMenu", // 获取登录菜单
            };
            if (defaultRoutes.Contains(routeName)) return true;

            // 获取用户权限集合（按钮或API接口）
            var permissionList = await App.GetService<SysMenuService>().GetPermCodeList();
            var allPermissionList = await App.GetService<SysMenuService>().GetAllPermCodeList();

            // 检查授权（菜单中没有配置按钮权限，则不限制）
            return permissionList.Exists(p => p.Equals(routeName, System.StringComparison.CurrentCultureIgnoreCase)) ||
                allPermissionList.TrueForAll(p => !p.Equals(routeName, System.StringComparison.CurrentCultureIgnoreCase));
        }
    }
}