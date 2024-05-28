﻿using Application.CustomExceptions;
using Application.Dtos.Reviews;
using Application.Services.Entities;
using AutoMapper;
using Domain.Entities;
using Domain.Entities.Reviews;
using Domain.Interfaces;
using NSubstitute;
using Xunit;
using Assert = Xunit.Assert;

namespace UnitTests.Application.Services.Entities;

public class ReviewDtoServiceTests
{
    private readonly IMapper _mapper;
    private readonly IReviewRepository _repository;
    private readonly ReviewDtoService _reviewDtoService;

    public ReviewDtoServiceTests()
    {
        _mapper = Substitute.For<IMapper>();
        _repository = Substitute.For<IReviewRepository>();
        _reviewDtoService = new ReviewDtoService(_repository, _mapper);
    }

    [Fact]
    [Test]
    public async Task GetEntitiesAsync_ReturnsMappedReviews_WhenReviewsExist()
    {
        // Arrange
        var reviews = new List<Review> { new(1, "Great product!", "image.jpg", 1, new DateTime(), 1) };
        var reviewsDto = new List<ReviewDto> { new(1, "Great product!", "image.jpg", 1, new DateTime(), 1, new Product()) };

        _repository.GetEntitiesAsync().Returns(reviews);
        _mapper.Map<IEnumerable<ReviewDto>>(reviews).Returns(reviewsDto);

        // Act
        var result = await _reviewDtoService.GetEntitiesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(reviewsDto, result);
    }

    [Fact]
    [Test]
    public async Task GetEntitiesAsync_ReturnsEmptyList_WhenNoReviewsExist()
    {
        // Arrange
        _repository.GetEntitiesAsync().Returns(new List<Review>());

        // Act
        var result = await _reviewDtoService.GetEntitiesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    [Test]
    public async Task GetByIdAsync_ReturnsMappedReview_WhenReviewExists()
    {
        // Arrange
        var review = new Review(1, "Great product!", "image.jpg", 1, new DateTime(), 1);
        var reviewDto = new ReviewDto(1, "Great product!", "image.jpg", 1, new DateTime(), 1, new Product());

        _repository.GetByIdAsync(1).Returns(review);
        _mapper.Map<ReviewDto>(review).Returns(reviewDto);

        // Act
        var result = await _reviewDtoService.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(reviewDto, result);
    }


    [Fact]
    [Test]
    public async Task AddAsync_CallsRepositoryCreateAsync_WhenReviewIsValid()
    {
        // Arrange
        var review = new Review(1, "Great product!", "image.jpg", 1, new DateTime(), 1);
        var reviewDto = new ReviewDto(1, "Great product!", "image.jpg", 1, new DateTime(), 1, new Product());

        _mapper.Map<Review>(reviewDto).Returns(review);

        // Act
        await _reviewDtoService.AddAsync(reviewDto);

        // Assert
        await _repository.Received(1).CreateAsync(review);
    }

    [Fact]
    [Test]
    public async Task AddAsync_ThrowsReviewException_WhenReviewIsInvalid()
    {
        // Arrange
        var reviewDto = new ReviewDto(1, "Great product!", "image.jpg", 1, new DateTime(), 1, new Product());

        _mapper.Map<Review>(reviewDto).Returns((Review?)null);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ReviewException>(() => _reviewDtoService.AddAsync(reviewDto));
        Assert.Equal("An unexpected error occurred while processing the request.", ex.Message);
    }

    [Fact]
    [Test]
    public async Task UpdateAsync_CallsRepositoryUpdateAsync_WhenReviewIsValid()
    {
        // Arrange
        var review = new Review(1, "Update Comment", "image.jpg", 1, new DateTime(), 1);
        var reviewDto = new ReviewDto(1, "Update Comment", "image.jpg", 1, new DateTime(), 1, new Product());

        _mapper.Map<Review>(reviewDto).Returns(review);

        // Act
        await _reviewDtoService.UpdateAsync(reviewDto);

        // Assert
        await _repository.Received(1).UpdateAsync(review);
    }

    [Fact]
    [Test]
    public async Task UpdateAsync_ThrowsReviewException_WhenReviewIsInvalid()
    {
        // Arrange
        var reviewDto = new ReviewDto(1, "Update Comment", "image.jpg", 1, new DateTime(), 1, new Product());

        _mapper.Map<Review>(reviewDto).Returns((Review?)null);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ReviewException>(() => _reviewDtoService.UpdateAsync(reviewDto));
        Assert.Equal("An unexpected error occurred while processing the request.", ex.Message);
    }

    [Fact]
    [Test]
    public async Task DeleteAsync_CallsRepositoryDeleteAsync_WhenReviewExists()
    {
        // Arrange
        var review = new Review(1, "Update Comment", "image.jpg", 1, new DateTime(), 1);

        _repository.GetByIdAsync(1).Returns(review);

        // Act
        await _reviewDtoService.DeleteAsync(1);

        // Assert
        await _repository.Received(1).DeleteAsync(review);
    }
}