//----------------------------------------------------------------------------
//
//  Copyright (C) 2004-2013 by EMGU. All rights reserved.
//
//----------------------------------------------------------------------------

#include "umat_c.h"

cv::UMat* cvUMatCreate()
{
   return new cv::UMat();
}

void cvUMatUseCustomAllocator(cv::UMat* mat, MatAllocateCallback allocator, MatDeallocateCallback deallocator, cv::MatAllocator** matAllocator, cv::MatAllocator** oclAllocator)
{
   *matAllocator = emguMatAllocatorCreate(allocator, deallocator);
   *oclAllocator = mat->getStdAllocator(*matAllocator);
   
   if (*oclAllocator == *matAllocator)
   {
      *oclAllocator = 0;
      mat->allocator = *matAllocator;
   } else
   {
      mat->allocator = *oclAllocator;
   }
}
void cvUMatCreateData(cv::UMat* mat, int row, int cols, int type)
{
   mat->create(row, cols, type);
}
cv::UMat* cvUMatCreateFromROI(cv::UMat* mat, CvRect* roi)
{
   return new cv::UMat(*mat, *roi);
}
void cvUMatRelease(cv::UMat** mat)
{
   delete *mat;
   *mat = 0;
}
emgu::size cvUMatGetSize(cv::UMat* mat)
{
   emgu::size s;
   s.width = mat->cols;
   s.height = mat->rows;
   return s;
}
void cvUMatCopyTo(cv::UMat* mat, cv::_OutputArray* m, cv::_InputArray* mask)
{
   if (mask)
   {
      CV_Error(0, "This is not implemented");
      //mat->copyTo(*m, *mask);
   }
   else
      mat->copyTo(*m);
}

int cvUMatGetElementSize(cv::UMat* mat)
{
   return static_cast<int>( mat->elemSize());
}

int cvUMatGetChannels(cv::UMat* mat)
{
   return mat->channels();
}

bool cvUMatIsEmpty(cv::UMat* mat)
{
   return mat->empty();
}

void cvUMatSetTo(cv::UMat* mat, cv::_InputArray* value, cv::_InputArray* mask)
{
   mat->setTo(*value, mask ? *mask : (cv::InputArray) cv::noArray());
}