using System;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using DmcSocial.Interfaces;
using System.Collections.Generic;
using DmcSocial.Models;
using DmcSocial.API.Models;
using System.Linq;
using DmcSocial.Repositories;

namespace DmcSocial.API
{
  [Route("comment")]
  [ApiController]
  public class CommentController : ControllerBase
  {
    private readonly ICommentRepository _repo;
    private readonly Authenticate _auth;
    public CommentController(Authenticate auth, ICommentRepository repo)
    {
      _repo = repo;
      _auth = auth;
    }

    [HttpGet]
    [Route("post/{postId}")]
    public async Task<ActionResult<List<CommentResponse>>> GetPostComments(int postId, int offset, int limit)
    {
      var comments = await _repo.GetPostComments(postId, new CommentListParams(offset, limit));
      return comments.Select(u => new CommentResponse(u)).ToList();
    }

    [HttpGet]
    [Route("post/{postId}/count")]
    public async Task<ActionResult<int>> GetPostCommentsCount(int postId)
    {
      var count = await _repo.GetPostCommentsCount(postId);
      return count;
    }

    [HttpGet]
    [Route("{commentId}/comments")]
    public async Task<ActionResult<List<CommentResponse>>> GetSubPostComments(int commentId, int offset, int limit)
    {
      var comments = await _repo.GetSubPostComments(commentId, new CommentListParams(offset, limit));
      return comments.Select(u => new CommentResponse(u)).ToList();
    }

    [HttpGet]
    [Route("{commentId}/comments/count")]
    public async Task<ActionResult<int>> GetSubPostCommentsCount(int commentId)
    {
      var count = await _repo.GetSubPostCommentsCount(commentId);
      return count;
    }

    [HttpPost]
    public async Task<ActionResult<CommentResponse>> CreateComment(CreateComment req)
    {
      var entity = await _repo.CreatePostComment(req.PostId, req.CommentId, req.Content, _auth.GetUser());
      return new CommentResponse(entity);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<CommentResponse>> UpdateComment(int id, UpdateComment payload)
    {
      var comment = await _repo.UpdatePostComment(id, payload.Content, _auth.GetUser());
      return new CommentResponse(comment);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteComment(int id)
    {
      var comment = await _repo.GetPostCommentById(id);
      if (comment == null) return NotFound();
      await _repo.DeleteComment(comment);
      return Ok();
    }
  }
}