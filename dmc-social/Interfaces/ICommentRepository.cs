using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DmcSocial.Models;

namespace DmcSocial.Interfaces
{
  public interface ICommentRepository
  {
    Task<PostComment> CreatePostComment(int postId, int? commentId, string content, string by);
    Task<List<PostComment>> GetPostComments(int postId, GetListParams<PostComment> param);
    Task<List<PostComment>> GetSubPostComments(int commentId, GetListParams<PostComment> param);
    Task<int> GetPostCommentsCount(int postId);
    Task<int> GetSubPostCommentsCount(int commentId);
    Task DeleteComment(PostComment comment);
    Task<PostComment> GetPostCommentById(int id);
    Task<PostComment> UpdatePostComment(int id, string content, string by);
  }
}