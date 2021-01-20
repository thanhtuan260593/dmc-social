using System;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ThanhTuan.Blogs.Interfaces;
using System.Collections.Generic;
using ThanhTuan.Blogs.Entities;
using ThanhTuan.Blogs.API.Models;
using System.Linq;
using ThanhTuan.Blogs.Repositories;

namespace ThanhTuan.Blogs.API
{
  [Route("me")]
  [ApiController]
  public class MeController : ControllerBase
  {
    private readonly ICommentRepository _comment;
    private readonly IPostRepository _post;
    private readonly Authenticate _auth;

    public MeController(ICommentRepository comment, IPostRepository post, Authenticate auth)
    {
      _comment = comment;
      _post = post;
      _auth = auth;
    }

    private async Task<ActionResult> HandlePostOwner(int id)
    {
      var post = await _post.GetPostById(id, false);
      if (post == null) return NotFound();
      if (post.CreatedBy != _auth.GetUser()) return Unauthorized();
      return null;
    }

    private async Task<ActionResult> HandleCommentOwner(int id)
    {
      var comment = await _comment.GetPostCommentById(id);
      if (comment == null) return NotFound();
      if (comment.CreatedBy != _auth.GetUser()) return Unauthorized();
      return null;
    }

    #region User Post API
    [HttpPost("post")]
    public async Task<ActionResult<PostResponse>> CreatePost(CreatePost req)
    {
      var post = req.ToEntity();
      var postTags = post.PostTags;
      await _post.CreatePost(post, _auth.GetUser());
      foreach (var postTag in postTags)
      {
        await _post.AddTag(post, postTag.TagId, _auth.GetUser());
      }
      return Ok(new PostResponse(post));
    }

    [HttpPut("post/{id}")]
    public async Task<ActionResult<PostResponse>> UpdatePostContent(int id, UpdatePostContent req)
    {
      var result = await HandlePostOwner(id);
      if (result != null) return result;
      var post = await _post.UpdatePostContent(id, req.Title, req.Subtitle, req.CoverImageURL, req.Content, _auth.GetUser());
      return Ok(new PostResponse(post));
    }

    [HttpPut("post/{id}/config")]
    public async Task<ActionResult<PostResponse>> UpdatePostConfig(int id, UpdatePostConfig req)
    {
      var result = await HandlePostOwner(id);
      if (result != null) return result;
      var post = await _post.UpdatePostConfig(id, req.PostRestrictionType, req.AccessUsers, req.CanComment);
      return Ok(new PostResponse(post));
    }

    [HttpDelete("post/{id}")]
    public async Task<ActionResult> DeletePost(int id)
    {
      var result = await HandlePostOwner(id);
      if (result != null) return result;
      var post = await _post.GetPostById(id, false);
      if (post == null) return NotFound();
      if (post.CreatedBy != _auth.GetUser()) return Unauthorized();
      await _post.DeletePost(id, _auth.GetUser());
      return Ok();
    }

    [HttpPost]
    [Route("post/{postId}/tag")]
    public async Task<ActionResult> AddTag(int postId, string tag)
    {
      var result = await HandlePostOwner(postId);
      if (result != null) return result;
      var post = await _post.GetPostById(postId, false);
      if (post == null)
        return NotFound();
      await _post.AddTag(post, tag, _auth.GetUser());
      return Ok();
    }

    [HttpDelete]
    [Route("post/{postId}/tag")]
    public async Task<ActionResult> DeleteTag(int postId, string tag)
    {
      var result = await HandlePostOwner(postId);
      if (result != null) return result;
      var post = await _post.GetPostById(postId, false);
      if (post == null)
        return NotFound();
      await _post.RemoveTag(post, tag, _auth.GetUser());
      return Ok();
    }
    #endregion

    #region User Comments API
    [HttpPost("comment")]
    public async Task<ActionResult<CommentResponse>> CreateComment(CreateComment req)
    {
      var entity = await _comment.CreatePostComment(req.PostId, req.CommentId, req.Content, _auth.GetUser());
      return new CommentResponse(entity);
    }

    [HttpPut("comment/{id}")]
    public async Task<ActionResult<CommentResponse>> UpdateComment(int id, UpdateComment payload)
    {
      var result = await HandleCommentOwner(id);
      if (result != null) return result;
      var comment = await _comment.UpdatePostComment(id, payload.Content, _auth.GetUser());
      return new CommentResponse(comment);
    }

    [HttpDelete("comment/{id}")]
    public async Task<ActionResult> DeleteComment(int id)
    {
      var result = await HandleCommentOwner(id);
      if (result != null) return result;
      var comment = await _comment.GetPostCommentById(id);
      if (comment == null) return NotFound();
      await _comment.DeleteComment(comment, _auth.GetUser());
      return Ok();
    }
    #endregion
  }
}
