﻿//------------------------------------------------------------------------------
//  此代码版权声明为全文件覆盖，如有原作者特别声明，会在下方手动补充
//  此代码版权（除特别声明外的代码）归作者本人Diego所有
//  源代码使用协议遵循本仓库的开源协议及附加协议
//  Gitee源代码仓库：https://gitee.com/diego2098/ThingsGateway
//  Github源代码仓库：https://github.com/kimdiego2098/ThingsGateway
//  使用文档：https://kimdiego2098.github.io/
//  QQ群：605534569
//------------------------------------------------------------------------------

using Mapster;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ThingsGateway.Admin.Application;

[Route("openapi/auth")]
[LoggingMonitor]
public class OpenApiController : ControllerBase
{
    private readonly IAuthService _authService;

    public OpenApiController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<OpenApiLoginOutput> LoginAsync([FromBody] OpenApiLoginInput input)
    {
        var output = await _authService.LoginAsync(input.Adapt<LoginInput>(), false);

        var openApiLoginOutput = output.Adapt<OpenApiLoginOutput>();

        return openApiLoginOutput;
    }

    [HttpPost("logout")]
    [Authorize]
    [IgnoreRolePermission]
    public async Task LogoutAsync()
    {
        await _authService.LoginOutAsync();
    }
}
