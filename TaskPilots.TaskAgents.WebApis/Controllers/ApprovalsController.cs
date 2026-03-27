using Microsoft.AspNetCore.Mvc;
using TaskPilots.TaskAgents.Core;
using TaskPilots.TaskAgents.WebApis.Services;

namespace TaskPilots.TaskAgents.WebApis.Controllers;

[ApiController]
[Route("api/approvals")]
public sealed class ApprovalsController(ApprovalApplicationService applicationService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<ApprovalListItemDto>>(StatusCodes.Status200OK)]
    public Task<IReadOnlyList<ApprovalListItemDto>> GetApprovals(CancellationToken cancellationToken)
        => applicationService.GetApprovalsAsync(cancellationToken);

    [HttpPost("{approvalId}/approve")]
    [ProducesResponseType<ApprovalListItemDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApprovalListItemDto>> Approve(
        string approvalId,
        [FromBody] ApprovalDecisionRequest request,
        CancellationToken cancellationToken)
    {
        var approval = await applicationService.ApproveAsync(approvalId, request, cancellationToken);
        if (approval is null)
        {
            return NotFound(new ApiErrorResponse("Approval not found.", "approval_not_found"));
        }

        return Ok(approval);
    }

    [HttpPost("{approvalId}/reject")]
    [ProducesResponseType<ApprovalListItemDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApprovalListItemDto>> Reject(
        string approvalId,
        [FromBody] ApprovalDecisionRequest request,
        CancellationToken cancellationToken)
    {
        var approval = await applicationService.RejectAsync(approvalId, request, cancellationToken);
        if (approval is null)
        {
            return NotFound(new ApiErrorResponse("Approval not found.", "approval_not_found"));
        }

        return Ok(approval);
    }
}
