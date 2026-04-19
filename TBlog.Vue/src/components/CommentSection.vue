<template>
    <div class="comment-section">
        <!-- 评论输入框 -->
        <div class="comment-input-section mb-4">
            <div class="comment-input-header">
                <h5>发表评论</h5>
            </div>
            <div class="comment-input-content">
                <textarea 
                    v-model="commentContent" 
                    class="form-control" 
                    rows="3" 
                    placeholder="写下你的评论..."
                    maxlength="2000"
                    :disabled="!isLoggedIn"
                ></textarea>
                <div class="comment-input-footer d-flex justify-content-between align-items-center mt-2">
                    <small class="text-muted">{{ commentContent.length }}/2000</small>
                    <button 
                        type="button" 
                        class="btn btn-primary btn-sm" 
                        @click="submitComment"
                        :disabled="!commentContent.trim() || !isLoggedIn"
                    >
                        发表评论
                    </button>
                </div>
            </div>
        </div>

        <!-- 评论列表 -->
        <div class="comment-list-section">
            <div class="comment-list-header">
                <h5>评论 ({{ totalComments }})</h5>
            </div>
            
            <div v-if="comments.length === 0" class="no-comments text-center py-4">
                <p class="text-muted">暂无评论，快来发表第一条评论吧！</p>
            </div>

            <div v-else class="comment-list">
                <div 
                    v-for="comment in comments" 
                    :key="comment.Id" 
                    class="comment-item"
                >
                    <!-- 主评论 -->
                    <div class="comment-main">
                        <div class="comment-avatar">
                            <img :src="comment.User.HeadImgUrl || '/default-avatar.png'" alt="头像" />
                        </div>
                        <div class="comment-content">
                            <div class="comment-header">
                                <span class="comment-author">{{ comment.User.UserName }}</span>
                                <span class="comment-time">{{ formatTime(comment.CDate) }}</span>
                            </div>
                            <div class="comment-text">{{ comment.Content }}</div>
                            <div class="comment-actions">
                                <button 
                                    type="button" 
                                    class="btn btn-link btn-sm p-0 me-3"
                                    @click="likeComment(comment)"
                                    :class="{ 'text-primary': comment.IsLiked }"
                                >
                                    <i class="bi" :class="comment.IsLiked ? 'bi-hand-thumbs-up-fill' : 'bi-hand-thumbs-up'"></i>
                                    {{ comment.LikeCount }}
                                </button>
                                <button 
                                    type="button" 
                                    class="btn btn-link btn-sm p-0 me-3"
                                    @click="showReplyInput(comment)"
                                >
                                    <i class="bi bi-reply"></i>
                                    回复
                                </button>
                                <button 
                                    v-if="canDeleteComment(comment)"
                                    type="button" 
                                    class="btn btn-link btn-sm p-0 text-danger"
                                    @click="deleteComment(comment)"
                                >
                                    <i class="bi bi-trash"></i>
                                    删除
                                </button>
                            </div>
                        </div>
                    </div>

                    <!-- 回复输入框 -->
                    <div v-if="comment.showReplyInput" class="reply-input-section mt-3">
                        <div class="reply-input-content">
                            <textarea 
                                v-model="comment.replyContent" 
                                class="form-control" 
                                rows="2" 
                                placeholder="回复评论..."
                                maxlength="2000"
                            ></textarea>
                            <div class="reply-input-footer d-flex justify-content-between align-items-center mt-2">
                                <small class="text-muted">{{ comment.replyContent ? comment.replyContent.length : 0 }}/2000</small>
                                <div>
                                    <button 
                                        type="button" 
                                        class="btn btn-secondary btn-sm me-2"
                                        @click="cancelReply(comment)"
                                    >
                                        取消
                                    </button>
                                    <button 
                                        type="button" 
                                        class="btn btn-primary btn-sm"
                                        @click="submitReply(comment)"
                                        :disabled="!comment.replyContent.trim()"
                                    >
                                        回复
                                    </button>
                                </div>
                            </div>
                        </div>
                    </div>

                    <!-- 子评论列表 -->
                    <div v-if="comment.Children && comment.Children.length > 0" class="comment-children">
                        <div 
                            v-for="(childComment, index) in comment.Children" 
                            :key="childComment.Id"
                            class="comment-child"
                            v-show="!comment.IsCollapsed || index < 3"
                        >
                            <div class="comment-avatar">
                                <img :src="childComment.User.HeadImgUrl || '/default-avatar.png'" alt="头像" />
                            </div>
                            <div class="comment-content">
                                <div class="comment-header">
                                    <span class="comment-author">{{ childComment.User.UserName }}</span>
                                    <span v-if="childComment.Parent" class="comment-reply-to">
                                        回复 @{{ childComment.Parent.User.UserName }}
                                    </span>
                                    <span class="comment-time">{{ formatTime(childComment.CDate) }}</span>
                                </div>
                                <div class="comment-text">{{ childComment.Content }}</div>
                                <div class="comment-actions">
                                    <button 
                                        type="button" 
                                        class="btn btn-link btn-sm p-0 me-3"
                                        @click="likeComment(childComment)"
                                        :class="{ 'text-primary': childComment.IsLiked }"
                                    >
                                        <i class="bi" :class="childComment.IsLiked ? 'bi-hand-thumbs-up-fill' : 'bi-hand-thumbs-up'"></i>
                                        {{ childComment.LikeCount }}
                                    </button>
                                    <button 
                                        type="button" 
                                        class="btn btn-link btn-sm p-0 me-3"
                                        @click="showReplyInput(comment, childComment)"
                                    >
                                        <i class="bi bi-reply"></i>
                                        回复
                                    </button>
                                    <button 
                                        v-if="canDeleteComment(childComment)"
                                        type="button" 
                                        class="btn btn-link btn-sm p-0 text-danger"
                                        @click="deleteComment(childComment)"
                                    >
                                        <i class="bi bi-trash"></i>
                                        删除
                                    </button>
                                </div>
                            </div>
                        </div>

                        <!-- 折叠按钮 -->
                        <div v-if="comment.ShowCollapse" class="comment-collapse">
                            <button 
                                type="button" 
                                class="btn btn-link btn-sm"
                                @click="toggleCollapse(comment)"
                            >
                                {{ comment.IsCollapsed ? `展开 ${comment.ReplyCount} 条回复` : '收起回复' }}
                            </button>
                        </div>
                    </div>
                </div>
            </div>

            <!-- 加载更多 -->
            <div v-if="hasMore" class="load-more text-center py-3">
                <button 
                    type="button" 
                    class="btn btn-outline-primary"
                    @click="loadMoreComments"
                    :disabled="loading"
                >
                    {{ loading ? '加载中...' : '加载更多评论' }}
                </button>
            </div>
        </div>
    </div>
</template>

<script>
import { 
    GetCommentList, 
    CreateComment, 
    LikeComment, 
    DeleteComment,
    GetChildComments 
} from '../assets/js/interface.js';

export default {
    name: 'CommentSection',
    props: {
        acticleId: {
            type: [String, Number],
            required: true
        }
    },
    data() {
        return {
            comments: [],
            commentContent: '',
            totalComments: 0,
            currentPage: 1,
            pageSize: 10,
            hasMore: true,
            loading: false,
            isLoggedIn: false
        }
    },
    methods: {
        async loadComments(page = 1) {
            try {
                this.loading = true;
                const response = await GetCommentList(this.acticleId, page, this.pageSize);
                if (response.Status === 200) {
                    const newComments = response.Data.Data.map(comment => ({
                        ...comment,
                        showReplyInput: false,
                        replyContent: '',
                        IsCollapsed: comment.ShowCollapse
                    }));
                    
                    if (page === 1) {
                        this.comments = newComments;
                    } else {
                        this.comments.push(...newComments);
                    }
                    
                    this.totalComments = response.Data.TotalCount;
                    this.hasMore = response.Data.Data.length === this.pageSize;
                    this.currentPage = page;
                }
            } catch (error) {
                console.error('加载评论失败:', error);
                this.$toast.error('加载评论失败');
            } finally {
                this.loading = false;
            }
        },

        async submitComment() {
            if (!this.commentContent.trim()) return;
            
            try {
                const response = await CreateComment({
                    ActicleId: this.acticleId,
                    Content: this.commentContent.trim(),
                    ParentId: null
                });
                
                if (response.Status === 200) {
                    this.commentContent = '';
                    // 重新加载第一页评论
                    await this.loadComments(1);
                }
            } catch (error) {
                console.error('发表评论失败:', error);
                this.$toast.error('发表评论失败');
            }
        },

        async submitReply(comment) {
            if (!comment.replyContent.trim()) return;
            
            try {
                const response = await CreateComment({
                    ActicleId: this.acticleId,
                    Content: comment.replyContent.trim(),
                    ParentId: comment.Id
                });
                
                if (response.Status === 200) {
                    comment.replyContent = '';
                    comment.showReplyInput = false;
                    // 重新加载评论以更新回复数
                    await this.loadComments(this.currentPage);
                }
            } catch (error) {
                console.error('回复失败:', error);
                this.$toast.error('回复失败');
            }
        },

        showReplyInput(comment, parentComment = null) {
            comment.showReplyInput = true;
            comment.replyContent = '';
        },

        cancelReply(comment) {
            comment.showReplyInput = false;
            comment.replyContent = '';
        },

        async likeComment(comment) {
            try {
                const response = await LikeComment(comment.Id);
                if (response.Status === 200) {
                    comment.IsLiked = !comment.IsLiked;
                    comment.LikeCount += comment.IsLiked ? 1 : -1;
                }
            } catch (error) {
                console.error('点赞失败:', error);
                this.$toast.error('点赞失败');
            }
        },

        async deleteComment(comment) {
            if (!confirm('确定要删除这条评论吗？')) return;
            
            try {
                const response = await DeleteComment(comment.Id);
                if (response.Status === 200) {
                    this.$toast.success('删除成功');
                    // 重新加载评论
                    await this.loadComments(this.currentPage);
                }
            } catch (error) {
                console.error('删除失败:', error);
                this.$toast.error('删除失败');
            }
        },

        toggleCollapse(comment) {
            comment.IsCollapsed = !comment.IsCollapsed;
        },

        async loadMoreComments() {
            await this.loadComments(this.currentPage + 1);
        },

        formatTime(timeStr) {
            const date = new Date(timeStr);
            const now = new Date();
            const diff = now - date;
            
            const minutes = Math.floor(diff / 60000);
            const hours = Math.floor(diff / 3600000);
            const days = Math.floor(diff / 86400000);
            
            if (minutes < 1) return '刚刚';
            if (minutes < 60) return `${minutes}分钟前`;
            if (hours < 24) return `${hours}小时前`;
            if (days < 30) return `${days}天前`;
            
            return date.toLocaleDateString();
        },

        canDeleteComment(comment) {
            // 检查是否是当前用户的评论
            return this.isLoggedIn && this.isSelfByUserId(comment.User.Id);
        },

        getCurrentUserId() {
            // 从localStorage或token中获取当前用户ID
            const token = localStorage.getItem('token');
            if (token) {
                try {
                    const payload = JSON.parse(atob(token.split('.')[1]));
                    return payload.UserId;
                } catch (e) {
                    return null;
                }
            }
            return null;
        },

        checkLoginStatus() {
            const token = localStorage.getItem('token');
            console.log(this.Config.token);  
            this.isLoggedIn = this.Config.token!='';
        }
    },

    async mounted() {

        this.checkLoginStatus();
        await this.loadComments();
    }
}
</script>

<style scoped>
.comment-section {
    margin-top: 2rem;
    padding-top: 2rem;
    border-top: 1px solid #e9ecef;
}

.comment-input-section {
    background: #f8f9fa;
    border-radius: 8px;
    padding: 1rem;
}

.comment-input-header h5 {
    margin-bottom: 1rem;
    color: #495057;
}

.comment-list-header h5 {
    margin-bottom: 1rem;
    color: #495057;
}

.comment-item {
    margin-bottom: 1.5rem;
    padding-bottom: 1.5rem;
    border-bottom: 1px solid #e9ecef;
}

.comment-item:last-child {
    border-bottom: none;
}

.comment-main {
    display: flex;
    gap: 1rem;
}

.comment-avatar img {
    width: 40px;
    height: 40px;
    border-radius: 50%;
    object-fit: cover;
}

.comment-content {
    flex: 1;
}

.comment-header {
    margin-bottom: 0.5rem;
}

.comment-author {
    font-weight: 600;
    color: #495057;
    margin-right: 0.5rem;
}

.comment-reply-to {
    color: #6c757d;
    font-size: 0.9rem;
    margin-right: 0.5rem;
}

.comment-time {
    color: #6c757d;
    font-size: 0.9rem;
}

.comment-text {
    color: #212529;
    line-height: 1.6;
    margin-bottom: 0.5rem;
}

.comment-actions {
    display: flex;
    align-items: center;
}

.comment-actions .btn-link {
    text-decoration: none;
    color: #6c757d;
}

.comment-actions .btn-link:hover {
    color: #495057;
}

.comment-children {
    margin-top: 1rem;
    margin-left: 3rem;
    padding-left: 1rem;
    border-left: 2px solid #e9ecef;
}

.comment-child {
    display: flex;
    gap: 0.75rem;
    margin-bottom: 1rem;
}

.comment-child .comment-avatar img {
    width: 32px;
    height: 32px;
}

.comment-collapse {
    text-align: center;
    margin-top: 0.5rem;
}

.reply-input-section {
    margin-left: 3rem;
    background: #f8f9fa;
    border-radius: 6px;
    padding: 1rem;
}

.no-comments {
    color: #6c757d;
}

.load-more {
    border-top: 1px solid #e9ecef;
}
</style> 