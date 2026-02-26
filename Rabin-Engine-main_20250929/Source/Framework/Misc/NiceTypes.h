/******************************************************************************/
/*!
\file		NiceTypes.h
\project	CS380/CS580 AI Framework
\author		Dustin Holmes
\summary	Convenience typedefs and structs

Copyright (C) 2018 DigiPen Institute of Technology.
Reproduction or disclosure of this file or its contents without the prior
written consent of DigiPen Institute of Technology is prohibited.
*/
/******************************************************************************/

#pragma once
#include "SimpleMath.h"

#define PI DirectX::XM_PI
#define TWO_PI DirectX::XM_2PI
#define HALF_PI DirectX::XM_PIDIV2
#define QTR_PI DirectX::XM_PIDIV4
#define ROOT2 1.414213562373095f

using Vec2 = DirectX::SimpleMath::Vector2;
using Vec3 = DirectX::SimpleMath::Vector3;
using Vec4 = DirectX::SimpleMath::Vector4;
using Mat4 = DirectX::SimpleMath::Matrix;
using Quat = DirectX::SimpleMath::Quaternion;
using Rect = DirectX::SimpleMath::Rectangle;
namespace Colors = DirectX::Colors;
using Callback = std::function<void(void)>;

extern const Vec3 globalUp;
extern const Vec3 globalRight;
extern const Vec3 globalForward;

template <typename T>
using Setter = std::function<void(const T &)>;

template <typename T>
using Getter = std::function<typename T(void)>;

using TextGetter = std::function<const std::wstring &(void)>;

struct WindowPos
{
    int x;
    int y;

    WindowPos operator+(const WindowPos &rhs) const
    {
        return WindowPos { x + rhs.x, y + rhs.y };
    }

    WindowPos operator-(const WindowPos &rhs) const
    {
        return WindowPos { x - rhs.x, y - rhs.y };
    }
};

struct GridPos
{
  GridPos(int r = 0, int c = 0) : row(r), col(c) { }
  GridPos(const GridPos& rhs) : row(rhs.row), col(rhs.col) { }
  GridPos & operator=(const GridPos& rhs)
  {
    row = rhs.row;
    col = rhs.col;
    return *this;
  }

  int row;
  int col;

  GridPos operator+(const GridPos& rhs)
  {
    return GridPos(row + rhs.row, col + rhs.col);
  }

  GridPos operator-(const GridPos& rhs)
  {
    return GridPos(row - rhs.row, col - rhs.col);
  }


  bool operator==(const GridPos &rhs) const
  {
      return row == rhs.row && col == rhs.col;
  }

  bool operator!=(const GridPos &rhs) const
  {
      return row != rhs.row || col != rhs.col;
  }

  static float Distance(GridPos  a, GridPos b)
  {
    if (a.row == b.row || a.col == b.col)
      return 1.0f;
    else
      return ROOT2;

  }

  static GridPos Clamp(GridPos pos, GridPos min, GridPos max)
  {
    pos.row = std::clamp(pos.row, min.row, max.row);
    pos.col = std::clamp(pos.col, min.col, max.col);

    return pos;
  }
};

struct WindowSize
{
    int width;
    int height;
};

// DXTK introduced the explicit specifier to the XMVECTORF32 conversion
// constructor for SimpleMath::Color in commit 5e62662, so we're using this
// workaround to avoid unnecessary code, as the official documentation suggests
// to pass in the v member every time instead, which isn't ideal:
// https://github.com/Microsoft/DirectXTK/wiki/Color
struct Color : public DirectX::SimpleMath::Color
{
    using DirectX::SimpleMath::Color::Color;
    Color(const DirectX::XMVECTORF32& F)
      : DirectX::SimpleMath::Color::Color(F)
    {
    }

    // avoid implicit conversion to float * when comparing with XMVECTORF32
    bool operator == (const DirectX::XMVECTORF32& F) const
    {
      return static_cast<DirectX::SimpleMath::Color>(*this) == DirectX::SimpleMath::Color::Color(F);
    }

    bool operator != (const DirectX::XMVECTORF32& F) const
    {
      return static_cast<DirectX::SimpleMath::Color>(*this) != DirectX::SimpleMath::Color::Color(F);
    }
};
