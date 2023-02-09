/*!
 * \file	sspartvalueshape.h
 * \author	CRI Middleware Co., Ltd.
*/
#ifndef SSPARTVALUESHAPE_H
#define SSPARTVALUESHAPE_H

#include <map>
#include "../sstypes.h"

#include "sspartvalue.h"
#include "../shape/ssshape.h"

namespace SpriteStudio
{
	/*!
	 * \class	SsPartValueShape
	 * \brief	シェイプパーツパラメータ
	*/
	class SsPartValueShape : public SsPartValue
	{
	public:
		static const SsString	TAG;	//!< デフォルトタグ

		/*!
		 * \enum	SsPartValueShape::Type
		 * \brief	タイプ
		*/
		enum Type
		{
			T_Unknown = 0,	//!< 不明
			T_Triangle = 3,	//!< 三角形
			T_Rectangle = 4,	//!< 矩形
			T_Arrow = 7,	//!< 矢印形
			T_Star = 10,	//!< 星形
		};

		static std::map<Type, SsString>	s_mapFromType;		//!< タイプから文字列への変換マップ
		static std::map<Type, SsString>	s_mapFromTypeName;	//!< タイプ名から文字列への変換マップ

		//! 有効なタイプのリストを取得
		/*!
		 * \return	有効なタイプのリスト
		*/
		static SsStringList getAvailableTypeList();

		//! 有効なタイプ名のリストを取得
		/*!
		 * \return	有効なタイプ名のリスト
		*/
		static SsStringList getAvailableTypeNameList();

		//! 文字列をタイプに変換
		/*!
		 * \param	strType		文字列
		 * \return	タイプ
		*/
		static Type toType(const SsString& strType);

		//! タイプを文字列に変換
		/*!
		 * \param	eType		タイプ
		 * \return	文字列
		*/
		static SsString fromType(Type eType);

		//! コンストラクタ
		SsPartValueShape();

		//! デストラクタ
		~SsPartValueShape() override;

		//! 複製
		/*!
		 * \return	パーツパラメータ
		*/
		SsPartValue* duplicate() override;

		//! タイプを設定
		/*!
		 * \param	eType		タイプ
		*/
		void setType(Type eType);

		//! マスクを設定
		/*!
		 * \param	bMask		マスク
		*/
		void setMask(bool bMask);

		//! サイズを設定
		/*!
		 * \param	size		サイズ
		*/
		void setSize(const SsSizeF& size);

		//! タイプを取得
		/*!
		 * \return	タイプ
		*/
		Type getType() const;

		//! マスクを取得
		/*!
		 * \return	マスク
		*/
		bool isMask() const;

		bool update();

		//! シェイプを取得
		/*!
		 * \return	シェイプ
		*/
		const SsShape* getShape();

	private:
		Type				m_eType;			//!< タイプ
		bool				m_bMask;			//!< マスク

		bool				m_bUpdate;			//!< 更新が必要かどうか

		SsShape* m_pShape;			//!< シェイプ

		//! マップを初期化
		static void initMap();

		//! シェイプを切替
		/*!
		 * \param	eType		タイプ
		*/
		void changeShape(Type eType);
	};

};

#endif // SSPARTVALUESHAPE_H
